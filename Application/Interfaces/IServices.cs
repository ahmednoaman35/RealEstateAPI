using RealEstateAPI.Application.DTOs.Auth;
using RealEstateAPI.Application.DTOs.Property;
using RealEstateAPI.Application.DTOs.Review;
using RealEstateAPI.Application.DTOs.Inquiry;
using RealEstateAPI.Application.DTOs.Common;

namespace RealEstateAPI.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request);
}

public interface IPropertyService
{
    Task<PropertyDto?> GetByIdAsync(Guid id);
    Task<PagedResponse<PropertyDto>> GetAllAsync(PropertyFilterDto filter);
    Task<PropertyDto> CreateAsync(CreatePropertyDto dto, Guid ownerId);
    Task<bool> UpdateAsync(Guid id, UpdatePropertyDto dto, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
    Task<List<PropertyDto>> GetMyPropertiesAsync(Guid ownerId);
}

public interface IReviewService
{
    Task<ReviewDto?> GetByIdAsync(Guid id);
    Task<PagedResponse<ReviewDto>> GetByPropertyIdAsync(Guid propertyId, int pageNumber, int pageSize);
    Task<ReviewDto> CreateAsync(CreateReviewDto dto, Guid userId);
    Task<bool> UpdateAsync(Guid id, UpdateReviewDto dto, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
}

public interface IFavoriteService
{
    Task<bool> AddToFavoritesAsync(Guid propertyId, Guid userId);
    Task<bool> RemoveFromFavoritesAsync(Guid propertyId, Guid userId);
    Task<PagedResponse<PropertyDto>> GetUserFavoritesAsync(Guid userId, int pageNumber, int pageSize);
    Task<bool> IsFavoriteAsync(Guid propertyId, Guid userId);
}

public interface IInquiryService
{
    Task<InquiryDto?> GetByIdAsync(Guid id);
    Task<PagedResponse<InquiryDto>> GetUserInquiriesAsync(Guid userId, int pageNumber, int pageSize);
    Task<PagedResponse<InquiryDto>> GetAllInquiriesAsync(int pageNumber, int pageSize);
    Task<InquiryDto> CreateAsync(CreateInquiryDto dto, Guid userId);
    Task<bool> UpdateStatusAsync(Guid id, UpdateInquiryStatusDto dto);
    Task<bool> DeleteAsync(Guid id, Guid userId);
}
