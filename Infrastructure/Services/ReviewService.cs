using AutoMapper;
using Microsoft.Extensions.Logging;
using RealEstateAPI.Application.DTOs.Common;
using RealEstateAPI.Application.DTOs.Review;
using RealEstateAPI.Application.Interfaces;
using RealEstateAPI.Domain.Entities;
using RealEstateAPI.Domain.Interfaces;
using RealEstateAPI.Infrastructure.Repositories.Interfaces;

namespace RealEstateAPI.Application.Services;

/// <summary>
/// Review service for property reviews and ratings
/// </summary>
public class ReviewService : IReviewService
{
    private readonly IBaseRepository<Review> _reviewRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(
        IBaseRepository<Review> reviewRepository,
        IPropertyRepository propertyRepository,
        IMapper mapper,
        ILogger<ReviewService> logger)
    {
        _reviewRepository = reviewRepository;
        _propertyRepository = propertyRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ReviewDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            return review == null ? null : _mapper.Map<ReviewDto>(review);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving review: {ReviewId}", id);
            throw;
        }
    }

    public async Task<PagedResponse<ReviewDto>> GetByPropertyIdAsync(Guid propertyId, int pageNumber, int pageSize)
    {
        try
        {
            var allReviews = await _reviewRepository.FindAsync(r => r.PropertyId == propertyId && !r.IsDeleted);
            var reviews = allReviews
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var reviewDtos = _mapper.Map<List<ReviewDto>>(reviews);
            
            return new PagedResponse<ReviewDto>(reviewDtos, pageNumber, pageSize, allReviews.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reviews for property: {PropertyId}", propertyId);
            throw;
        }
    }

    public async Task<ReviewDto> CreateAsync(CreateReviewDto dto, Guid userId)
    {
        try
        {
            // Check if user already reviewed this property
            var existingReview = await _reviewRepository.FindAsync(
                r => r.PropertyId == dto.PropertyId && r.UserId == userId && !r.IsDeleted);
            
            if (existingReview.Any())
            {
                throw new InvalidOperationException("You have already reviewed this property");
            }

            var review = _mapper.Map<Review>(dto);
            review.UserId = userId;

            var createdReview = await _reviewRepository.AddAsync(review);

            // Update property rating
            await UpdatePropertyRatingAsync(dto.PropertyId);

            _logger.LogInformation("Review created for property: {PropertyId} by user: {UserId}", dto.PropertyId, userId);
            return _mapper.Map<ReviewDto>(createdReview);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating review for property: {PropertyId}", dto.PropertyId);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateReviewDto dto, Guid userId)
    {
        try
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            
            if (review == null || review.IsDeleted)
            {
                return false;
            }

            // Check ownership
            if (review.UserId != userId)
            {
                _logger.LogWarning("Unauthorized update attempt for review: {ReviewId}", id);
                return false;
            }

            if (dto.Rating.HasValue) review.Rating = dto.Rating.Value;
            if (dto.Comment != null) review.Comment = dto.Comment;
            review.UpdatedAt = DateTime.UtcNow;

            await _reviewRepository.UpdateAsync(review);

            // Update property rating
            await UpdatePropertyRatingAsync(review.PropertyId);

            _logger.LogInformation("Review updated: {ReviewId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating review: {ReviewId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        try
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            
            if (review == null || review.IsDeleted)
            {
                return false;
            }

            // Check ownership
            if (review.UserId != userId)
            {
                _logger.LogWarning("Unauthorized delete attempt for review: {ReviewId}", id);
                return false;
            }

            review.IsDeleted = true;
            review.UpdatedAt = DateTime.UtcNow;
            
            await _reviewRepository.UpdateAsync(review);

            // Update property rating
            await UpdatePropertyRatingAsync(review.PropertyId);

            _logger.LogInformation("Review deleted: {ReviewId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting review: {ReviewId}", id);
            throw;
        }
    }

    private async Task UpdatePropertyRatingAsync(Guid propertyId)
    {
        var reviews = await _reviewRepository.FindAsync(r => r.PropertyId == propertyId && !r.IsDeleted);
        var reviewList = reviews.ToList();

        var property = await _propertyRepository.GetByIdAsync(propertyId);
        if (property != null)
        {
            property.TotalReviews = reviewList.Count;
            property.AverageRating = reviewList.Any() ? reviewList.Average(r => r.Rating) : 0;
            await _propertyRepository.UpdateAsync(property);
        }
    }
}
