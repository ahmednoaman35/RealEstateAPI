using Microsoft.EntityFrameworkCore;
using RealEstateAPI.Application.DTOs.Property;
using RealEstateAPI.Domain.Entities;
using RealEstateAPI.Infrastructure.Data;
using RealEstateAPI.Infrastructure.Repositories.Interfaces;

namespace RealEstateAPI.Infrastructure.Repositories;

/// <summary>
/// Property repository with filtering, sorting, and pagination
/// </summary>
public class PropertyRepository : BaseRepository<Property>, IPropertyRepository
{
    public PropertyRepository(ApplicationDbContext context) : base(context)
    {
    }



    public async Task<Property?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.Owner)
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Property>> GetByOwnerIdAsync(Guid ownerId)
    {
        return await _context.Properties
            .Where(p => p.OwnerId == ownerId && !p.IsDeleted)
            .Include(p => p.Images)
            .ToListAsync();
    }
    public async Task<(List<Property> Properties, int TotalCount)> GetFilteredPropertiesAsync(PropertyFilterDto filter)
    {
        var query = _dbSet
            .Include(p => p.Owner)
            .Include(p => p.Images)
            .AsQueryable();

        // Apply filters
        if (filter.MinPrice.HasValue)
            query = query.Where(p => p.Price >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);

        if (!string.IsNullOrEmpty(filter.Location))
            query = query.Where(p => p.Location.Contains(filter.Location));

        if (!string.IsNullOrEmpty(filter.City))
            query = query.Where(p => p.City.Contains(filter.City));

        if (filter.PropertyType.HasValue)
            query = query.Where(p => p.PropertyType == filter.PropertyType.Value);

        if (filter.Status.HasValue)
            query = query.Where(p => p.Status == filter.Status.Value);

        if (filter.MinBedrooms.HasValue)
            query = query.Where(p => p.Bedrooms >= filter.MinBedrooms.Value);

        if (filter.MinBathrooms.HasValue)
            query = query.Where(p => p.Bathrooms >= filter.MinBathrooms.Value);

        if (filter.MinSquareFeet.HasValue)
            query = query.Where(p => p.SquareFeet >= filter.MinSquareFeet.Value);

        if (filter.HasParking.HasValue && filter.HasParking.Value)
            query = query.Where(p => p.HasParking);

        if (filter.HasGarden.HasValue && filter.HasGarden.Value)
            query = query.Where(p => p.HasGarden);

        if (filter.HasPool.HasValue && filter.HasPool.Value)
            query = query.Where(p => p.HasPool);

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = ApplySorting(query, filter.SortBy, filter.SortDescending);

        // Apply pagination
        var properties = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (properties, totalCount);
    }

    private IQueryable<Property> ApplySorting(IQueryable<Property> query, string? sortBy, bool descending)
    {
        return sortBy?.ToLower() switch
        {
            "price" => descending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
            "created" => descending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            "rating" => descending ? query.OrderByDescending(p => p.AverageRating) : query.OrderBy(p => p.AverageRating),
            _ => query.OrderByDescending(p => p.CreatedAt) // Default sorting
        };
    }
}
