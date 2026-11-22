using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using RealEstateAPI.Application.DTOs.Common;
using RealEstateAPI.Application.DTOs.Property;
using RealEstateAPI.Application.Interfaces;
using RealEstateAPI.Domain.Entities;
using RealEstateAPI.Domain.Enums;
using RealEstateAPI.Infrastructure.Repositories.Interfaces;
using System;
using System.Linq;

namespace RealEstateAPI.Application.Services;

/// <summary>
/// Property service handling CRUD operations and filtering
/// </summary>
public class PropertyService : IPropertyService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PropertyService> _logger;

    public PropertyService(
        IPropertyRepository propertyRepository,
        IMapper mapper,
        ILogger<PropertyService> logger,
        IWebHostEnvironment webHostEnvironment)
    {
        _propertyRepository = propertyRepository;
        _mapper = mapper;
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
        _mapper = mapper;
        _logger = logger;
    }



    public async Task<List<PropertyDto>> GetMyPropertiesAsync(Guid ownerId)
    {
        var properties = await _propertyRepository.GetByOwnerIdAsync(ownerId);

        return _mapper.Map<List<PropertyDto>>(properties);
    }
    public async Task<PropertyDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var property = await _propertyRepository.GetByIdWithDetailsAsync(id);
            if (property == null || property.IsDeleted)
            {
                return null;
            }

            return _mapper.Map<PropertyDto>(property);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving property with ID: {PropertyId}", id);
            throw;
        }
    }



    public async Task<PagedResponse<PropertyDto>> GetAllAsync(PropertyFilterDto filter)
    {
        try
        {
            var (properties, totalCount) = await _propertyRepository.GetFilteredPropertiesAsync(filter);
            
            var propertyDtos = _mapper.Map<List<PropertyDto>>(properties);
            
            return new PagedResponse<PropertyDto>(
                propertyDtos,
                filter.PageNumber,
                filter.PageSize,
                totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving filtered properties");
            throw;
        }
    }


    public async Task<PropertyDto> CreateAsync(CreatePropertyDto dto, Guid ownerId)
    {
        var property = _mapper.Map<Property>(dto);
        property.OwnerId = ownerId;

        property.Images = new List<PropertyImage>();

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var allowedContentTypes = new[] { "image/jpeg", "image/png" };

        int index = 0;

        if (dto.Images != null && dto.Images.Any())
        {
            foreach (var file in dto.Images)
            {
                // basic validation
                if (file == null || file.Length == 0)
                    throw new Exception("Image is required");

                if (file.Length > 10 * 1024 * 1024)
                    throw new Exception("Image size must not exceed 10MB");

                if (string.IsNullOrWhiteSpace(file.FileName))
                    throw new Exception("Invalid file name");

                // extension check
                var extension = Path.GetExtension(file.FileName)
                    .Trim()
                    .ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                    throw new Exception("Only JPG, JPEG, PNG allowed");

                // content type check
                if (!allowedContentTypes.Contains(file.ContentType.ToLower()))
                    throw new Exception("Invalid image type");

                // create folder
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "properties");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // unique file name
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // add image entity
                property.Images.Add(new PropertyImage
                {
                    ImageUrl = $"/properties/{fileName}",
                    DisplayOrder = index,
                    IsPrimary = index == 0
                });

                index++;
            }
        }

        var createdProperty = await _propertyRepository.AddAsync(property);

        var result = await _propertyRepository.GetByIdWithDetailsAsync(createdProperty.Id);

        return _mapper.Map<PropertyDto>(result);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdatePropertyDto dto, Guid userId)
    {
        try
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            
            if (property == null || property.IsDeleted)
            {
                _logger.LogWarning("Property not found: {PropertyId}", id);
                return false;
            }

            // Check ownership
            if (property.OwnerId != userId)
            {
                _logger.LogWarning("Unauthorized update attempt for property: {PropertyId} by user: {UserId}", id, userId);
                return false;
            }

            // Apply updates
            if (dto.Title != null) property.Title = dto.Title;
            if (dto.Description != null) property.Description = dto.Description;
            if (dto.Price.HasValue) property.Price = dto.Price.Value;
            if (dto.Location != null) property.Location = dto.Location;
            if (dto.City != null) property.City = dto.City;
            if (dto.Status.HasValue) property.Status = dto.Status.Value;
            if (dto.Bedrooms.HasValue) property.Bedrooms = dto.Bedrooms.Value;
            if (dto.Bathrooms.HasValue) property.Bathrooms = dto.Bathrooms.Value;
            if (dto.SquareFeet.HasValue) property.SquareFeet = dto.SquareFeet.Value;
            if (dto.HasParking.HasValue) property.HasParking = dto.HasParking.Value;
            if (dto.HasGarden.HasValue) property.HasGarden = dto.HasGarden.Value;
            if (dto.HasPool.HasValue) property.HasPool = dto.HasPool.Value;

            property.UpdatedAt = DateTime.UtcNow;

            await _propertyRepository.UpdateAsync(property);
            
            _logger.LogInformation("Property updated successfully: {PropertyId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating property: {PropertyId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        try
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            
            if (property == null || property.IsDeleted)
            {
                return false;
            }

            // Check ownership
            if (property.OwnerId != userId)
            {
                _logger.LogWarning("Unauthorized delete attempt for property: {PropertyId} by user: {UserId}", id, userId);
                return false;
            }

            // Soft delete
            property.IsDeleted = true;
            property.UpdatedAt = DateTime.UtcNow;
            
            await _propertyRepository.UpdateAsync(property);
            
            _logger.LogInformation("Property deleted successfully: {PropertyId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting property: {PropertyId}", id);
            throw;
        }
    }


    public async Task<bool> MarkAsSoldAsync(Guid propertyId, Guid userId)
    {
        try
        {
            var property = await _propertyRepository.GetByIdAsync(propertyId);

            if (property == null || property.IsDeleted)
                return false;

            // Ownership check
            if (property.OwnerId != userId)
                return false;

            // business rule: ·« Ì„ﬂ‰ »Ì⁄ ⁄ﬁ«— „ »«⁄ »«·›⁄·
            if (property.Status == PropertyStatus.Sold)
                return false;

            property.Status = PropertyStatus.Sold;
            property.UpdatedAt = DateTime.UtcNow;

            await _propertyRepository.UpdateAsync(property);

            _logger.LogInformation("Property marked as sold: {PropertyId}", propertyId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking property as sold: {PropertyId}", propertyId);
            throw;
        }
    }
}
