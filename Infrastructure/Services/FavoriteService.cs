using AutoMapper;
using Microsoft.Extensions.Logging;
using RealEstateAPI.Application.DTOs.Common;
using RealEstateAPI.Application.DTOs.Property;
using RealEstateAPI.Application.Interfaces;
using RealEstateAPI.Domain.Entities;
using RealEstateAPI.Domain.Interfaces;

namespace RealEstateAPI.Application.Services;

/// <summary>
/// Favorite service for managing user's favorite properties
/// </summary>
public class FavoriteService : IFavoriteService
{
    private readonly IBaseRepository<Favorite> _favoriteRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<FavoriteService> _logger;

    public FavoriteService(
        IBaseRepository<Favorite> favoriteRepository,
        IMapper mapper,
        ILogger<FavoriteService> logger)
    {
        _favoriteRepository = favoriteRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<bool> AddToFavoritesAsync(Guid propertyId, Guid userId)
    {
        try
        {
            // Check if already favorited
            var existing = await _favoriteRepository.FindAsync(
                f => f.PropertyId == propertyId && f.UserId == userId && !f.IsDeleted);

            if (existing.Any())
            {
                _logger.LogWarning("Property already in favorites: {PropertyId} for user: {UserId}", propertyId, userId);
                return false;
            }

            var favorite = new Favorite
            {
                PropertyId = propertyId,
                UserId = userId
            };

            await _favoriteRepository.AddAsync(favorite);
            
            _logger.LogInformation("Property added to favorites: {PropertyId} for user: {UserId}", propertyId, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding property to favorites: {PropertyId}", propertyId);
            throw;
        }
    }

    public async Task<bool> RemoveFromFavoritesAsync(Guid propertyId, Guid userId)
    {
        try
        {
            var favorite = (await _favoriteRepository.FindAsync(
                f => f.PropertyId == propertyId && f.UserId == userId && !f.IsDeleted)).FirstOrDefault();

            if (favorite == null)
            {
                return false;
            }

            favorite.IsDeleted = true;
            favorite.UpdatedAt = DateTime.UtcNow;
            
            await _favoriteRepository.UpdateAsync(favorite);
            
            _logger.LogInformation("Property removed from favorites: {PropertyId} for user: {UserId}", propertyId, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing property from favorites: {PropertyId}", propertyId);
            throw;
        }
    }

    public async Task<PagedResponse<PropertyDto>> GetUserFavoritesAsync(Guid userId, int pageNumber, int pageSize)
    {
        try
        {
            var allFavorites = await _favoriteRepository.FindAsync(f => f.UserId == userId && !f.IsDeleted);
            var favorites = allFavorites
                .OrderByDescending(f => f.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var propertyDtos = _mapper.Map<List<PropertyDto>>(favorites);
            
            return new PagedResponse<PropertyDto>(propertyDtos, pageNumber, pageSize, allFavorites.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving favorites for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> IsFavoriteAsync(Guid propertyId, Guid userId)
    {
        try
        {
            return await _favoriteRepository.ExistsAsync(
                f => f.PropertyId == propertyId && f.UserId == userId && !f.IsDeleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking favorite status for property: {PropertyId}", propertyId);
            throw;
        }
    }
}
