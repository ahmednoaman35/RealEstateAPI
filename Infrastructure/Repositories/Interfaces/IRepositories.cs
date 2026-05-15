using RealEstateAPI.Application.DTOs.Property;
using RealEstateAPI.Domain.Entities;
using RealEstateAPI.Domain.Interfaces;

namespace RealEstateAPI.Infrastructure.Repositories.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}

public interface IPropertyRepository : IBaseRepository<Property>
{
    Task<Property?> GetByIdWithDetailsAsync(Guid id);
    Task<(List<Property> Properties, int TotalCount)> GetFilteredPropertiesAsync(PropertyFilterDto filter);
    Task<List<Property>> GetByOwnerIdAsync(Guid ownerId);
}

