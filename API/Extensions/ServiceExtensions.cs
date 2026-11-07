using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RealEstateAPI.Application.Interfaces;
using RealEstateAPI.Application.Mappings;
using RealEstateAPI.Application.Services;
using RealEstateAPI.Application.Validators;
using RealEstateAPI.Domain.Entities;
using RealEstateAPI.Domain.Interfaces;
using RealEstateAPI.Infrastructure.Data;
using RealEstateAPI.Infrastructure.Identity;
using RealEstateAPI.Infrastructure.Repositories;
using RealEstateAPI.Infrastructure.Repositories.Interfaces;
using RealEstateAPI.Infrastructure.Services;
using RealEstateAPI.Services;
using System.Text;

namespace RealEstateAPI.API.Extensions;

/// <summary>
/// Service registration extensions
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Configure database context
    /// </summary>
    public static IServiceCollection AddDatabaseConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("RealEstateAPI.API")));

        return services;
    }

    /// <summary>
    /// Configure JWT authentication
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"] 
            ?? throw new InvalidOperationException("JWT Key not configured"));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; // Set to true in production
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }

    /// <summary>
    /// Register application services
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPropertyService, PropertyService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<IInquiryService, InquiryService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IWhatsAppService, WhatsAppService>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IInquiryRepository, InquiryRepository>();
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

        // Infrastructure
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }

    /// <summary>
    /// Configure AutoMapper
    /// </summary>
    public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        return services;
    }

    /// <summary>
    /// Configure FluentValidation
    /// </summary>
    public static IServiceCollection AddFluentValidationConfiguration(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        
        return services;
    }

    /// <summary>
    /// Configure CORS policy
    /// </summary>
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        return services;
    }
}
