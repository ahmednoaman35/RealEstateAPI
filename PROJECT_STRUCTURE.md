# Real Estate API - Project Structure

```
RealEstateAPI/
│
├── RealEstateAPI.sln
│
├── src/
│   ├── RealEstateAPI.Domain/                    # Core Business Logic & Entities
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── Property.cs
│   │   │   ├── Review.cs
│   │   │   ├── Favorite.cs
│   │   │   ├── Inquiry.cs
│   │   │   ├── PropertyImage.cs
│   │   │   └── BaseEntity.cs
│   │   ├── Enums/
│   │   │   ├── PropertyType.cs
│   │   │   ├── PropertyStatus.cs
│   │   │   └── InquiryStatus.cs
│   │   ├── Interfaces/
│   │   │   └── IBaseRepository.cs
│   │   └── RealEstateAPI.Domain.csproj
│   │
│   ├── RealEstateAPI.Application/               # Business Logic & DTOs
│   │   ├── DTOs/
│   │   │   ├── Auth/
│   │   │   │   ├── LoginRequestDto.cs
│   │   │   │   ├── RegisterRequestDto.cs
│   │   │   │   └── AuthResponseDto.cs
│   │   │   ├── Property/
│   │   │   │   ├── PropertyDto.cs
│   │   │   │   ├── CreatePropertyDto.cs
│   │   │   │   ├── UpdatePropertyDto.cs
│   │   │   │   └── PropertyFilterDto.cs
│   │   │   ├── Review/
│   │   │   │   ├── ReviewDto.cs
│   │   │   │   └── CreateReviewDto.cs
│   │   │   ├── Inquiry/
│   │   │   │   ├── InquiryDto.cs
│   │   │   │   └── CreateInquiryDto.cs
│   │   │   └── Common/
│   │   │       ├── ApiResponse.cs
│   │   │       └── PagedResponse.cs
│   │   ├── Interfaces/
│   │   │   ├── IAuthService.cs
│   │   │   ├── IPropertyService.cs
│   │   │   ├── IReviewService.cs
│   │   │   ├── IFavoriteService.cs
│   │   │   └── IInquiryService.cs
│   │   ├── Services/
│   │   │   ├── AuthService.cs
│   │   │   ├── PropertyService.cs
│   │   │   ├── ReviewService.cs
│   │   │   ├── FavoriteService.cs
│   │   │   └── InquiryService.cs
│   │   ├── Validators/
│   │   │   ├── RegisterRequestValidator.cs
│   │   │   ├── CreatePropertyValidator.cs
│   │   │   └── CreateReviewValidator.cs
│   │   ├── Mappings/
│   │   │   └── MappingProfile.cs
│   │   └── RealEstateAPI.Application.csproj
│   │
│   ├── RealEstateAPI.Infrastructure/            # Data Access & External Services
│   │   ├── Data/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   └── DbInitializer.cs
│   │   ├── Repositories/
│   │   │   ├── BaseRepository.cs
│   │   │   ├── PropertyRepository.cs
│   │   │   ├── UserRepository.cs
│   │   │   └── Interfaces/
│   │   │       ├── IPropertyRepository.cs
│   │   │       └── IUserRepository.cs
│   │   ├── Identity/
│   │   │   └── JwtTokenGenerator.cs
│   │   └── RealEstateAPI.Infrastructure.csproj
│   │
│   └── RealEstateAPI.API/                       # Web API Layer
│       ├── Controllers/
│       │   ├── AuthController.cs
│       │   ├── PropertiesController.cs
│       │   ├── ReviewsController.cs
│       │   ├── FavoritesController.cs
│       │   └── InquiriesController.cs
│       ├── Middleware/
│       │   └── GlobalExceptionHandlerMiddleware.cs
│       ├── Extensions/
│       │   ├── ServiceExtensions.cs
│       │   └── SwaggerExtensions.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── Program.cs
│       └── RealEstateAPI.API.csproj
│
└── README.md
```

## Layer Dependencies

- **Domain Layer**: No dependencies (Pure business logic)
- **Application Layer**: Depends on Domain
- **Infrastructure Layer**: Depends on Domain and Application
- **API Layer**: Depends on Application (and Infrastructure for DI registration)

## Key Design Patterns

1. **Repository Pattern**: Data access abstraction
2. **Service Pattern**: Business logic encapsulation
3. **DTO Pattern**: Data transfer and mapping
4. **Dependency Injection**: Loose coupling
5. **Middleware Pattern**: Cross-cutting concerns
6. **CQRS-lite**: Separation of read/write operations
