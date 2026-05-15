using AutoMapper;
using Microsoft.Extensions.Logging;
using RealEstateAPI.Application.DTOs.Common;
using RealEstateAPI.Application.DTOs.Inquiry;
using RealEstateAPI.Application.Interfaces;
using RealEstateAPI.Domain.Entities;
using RealEstateAPI.Domain.Interfaces;

namespace RealEstateAPI.Application.Services;

/// <summary>
/// Inquiry service for managing property inquiries
/// </summary>
public class InquiryService : IInquiryService
{
    private readonly IBaseRepository<Inquiry> _inquiryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<InquiryService> _logger;

    public InquiryService(
        IBaseRepository<Inquiry> inquiryRepository,
        IMapper mapper,
        ILogger<InquiryService> logger)
    {
        _inquiryRepository = inquiryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<InquiryDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var inquiry = await _inquiryRepository.GetByIdAsync(id);
            return inquiry == null ? null : _mapper.Map<InquiryDto>(inquiry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inquiry: {InquiryId}", id);
            throw;
        }
    }

    public async Task<PagedResponse<InquiryDto>> GetUserInquiriesAsync(Guid userId, int pageNumber, int pageSize)
    {
        try
        {
            var allInquiries = await _inquiryRepository.FindAsync(i => i.UserId == userId && !i.IsDeleted);
            var inquiries = allInquiries
                .OrderByDescending(i => i.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var inquiryDtos = _mapper.Map<List<InquiryDto>>(inquiries);
            
            return new PagedResponse<InquiryDto>(inquiryDtos, pageNumber, pageSize, allInquiries.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inquiries for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<PagedResponse<InquiryDto>> GetAllInquiriesAsync(int pageNumber, int pageSize)
    {
        try
        {
            var allInquiries = await _inquiryRepository.FindAsync(i => !i.IsDeleted);
            var inquiries = allInquiries
                .OrderByDescending(i => i.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var inquiryDtos = _mapper.Map<List<InquiryDto>>(inquiries);
            
            return new PagedResponse<InquiryDto>(inquiryDtos, pageNumber, pageSize, allInquiries.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all inquiries");
            throw;
        }
    }

    public async Task<InquiryDto> CreateAsync(CreateInquiryDto dto, Guid userId)
    {
        try
        {
            var inquiry = _mapper.Map<Inquiry>(dto);
            inquiry.UserId = userId;

            var createdInquiry = await _inquiryRepository.AddAsync(inquiry);

            _logger.LogInformation("Inquiry created for property: {PropertyId} by user: {UserId}", 
                dto.PropertyId, userId);
            
            return _mapper.Map<InquiryDto>(createdInquiry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating inquiry");
            throw;
        }
    }

    public async Task<bool> UpdateStatusAsync(Guid id, UpdateInquiryStatusDto dto)
    {
        try
        {
            var inquiry = await _inquiryRepository.GetByIdAsync(id);
            
            if (inquiry == null || inquiry.IsDeleted)
            {
                return false;
            }

            inquiry.Status = dto.Status;
            if (!string.IsNullOrEmpty(dto.AdminResponse))
            {
                inquiry.AdminResponse = dto.AdminResponse;
                inquiry.RespondedAt = DateTime.UtcNow;
            }
            inquiry.UpdatedAt = DateTime.UtcNow;

            await _inquiryRepository.UpdateAsync(inquiry);

            _logger.LogInformation("Inquiry status updated: {InquiryId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating inquiry status: {InquiryId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        try
        {
            var inquiry = await _inquiryRepository.GetByIdAsync(id);
            
            if (inquiry == null || inquiry.IsDeleted)
            {
                return false;
            }

            // Check ownership
            if (inquiry.UserId != userId)
            {
                _logger.LogWarning("Unauthorized delete attempt for inquiry: {InquiryId} by user: {UserId}", 
                    id, userId);
                return false;
            }

            inquiry.IsDeleted = true;
            inquiry.UpdatedAt = DateTime.UtcNow;
            
            await _inquiryRepository.UpdateAsync(inquiry);

            _logger.LogInformation("Inquiry deleted: {InquiryId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting inquiry: {InquiryId}", id);
            throw;
        }
    }
}
