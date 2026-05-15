using AutoMapper;
using RealEstateAPI.Application.DTOs.Auth;
using RealEstateAPI.Application.DTOs.Property;
using RealEstateAPI.Application.DTOs.Review;
using RealEstateAPI.Application.DTOs.Inquiry;
using RealEstateAPI.Domain.Entities;

namespace RealEstateAPI.Application.Mappings;

/// <summary>
/// AutoMapper profile for entity-DTO mappings
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User Mappings
        CreateMap<User, AuthResponseDto>()
            .ForMember(dest => dest.Token, opt => opt.Ignore())
            .ForMember(dest => dest.ExpiresAt, opt => opt.Ignore());

        CreateMap<RegisterRequestDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        // Property Mappings
        CreateMap<Property, PropertyDto>()
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => 
                src.Images.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).ToList()))
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => 
                $"{src.Owner.FirstName} {src.Owner.LastName}"));

        CreateMap<CreatePropertyDto, Property>()
            .ForMember(dest => dest.Images, opt => opt.Ignore());

        CreateMap<UpdatePropertyDto, Property>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Review Mappings
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.PropertyTitle, opt => opt.MapFrom(src => src.Property.Title))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => 
                $"{src.User.FirstName} {src.User.LastName}"));

        CreateMap<CreateReviewDto, Review>();

        // Inquiry Mappings
        CreateMap<Inquiry, InquiryDto>()
            .ForMember(dest => dest.PropertyTitle, opt => opt.MapFrom(src => src.Property.Title))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => 
                $"{src.User.FirstName} {src.User.LastName}"));

        CreateMap<CreateInquiryDto, Inquiry>();

        // Favorite Mappings
        CreateMap<Favorite, PropertyDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Property.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Property.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Property.Description))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Property.Price))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Property.Location))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Property.City))
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.Property.State))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Property.Address))
            .ForMember(dest => dest.PropertyType, opt => opt.MapFrom(src => src.Property.PropertyType))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Property.Status))
            .ForMember(dest => dest.Bedrooms, opt => opt.MapFrom(src => src.Property.Bedrooms))
            .ForMember(dest => dest.Bathrooms, opt => opt.MapFrom(src => src.Property.Bathrooms))
            .ForMember(dest => dest.SquareFeet, opt => opt.MapFrom(src => src.Property.SquareFeet))
            .ForMember(dest => dest.YearBuilt, opt => opt.MapFrom(src => src.Property.YearBuilt))
            .ForMember(dest => dest.HasParking, opt => opt.MapFrom(src => src.Property.HasParking))
            .ForMember(dest => dest.HasGarden, opt => opt.MapFrom(src => src.Property.HasGarden))
            .ForMember(dest => dest.HasPool, opt => opt.MapFrom(src => src.Property.HasPool))
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Property.AverageRating))
            .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => src.Property.TotalReviews))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => 
                src.Property.Images.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).ToList()))
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => 
                $"{src.Property.Owner.FirstName} {src.Property.Owner.LastName}"))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Property.CreatedAt));
    }
}
