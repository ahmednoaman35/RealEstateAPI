using Microsoft.AspNetCore.Http;
using RealEstateAPI.Domain.Enums;

namespace RealEstateAPI.Application.DTOs.Property;

public class PropertyDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Location { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public PropertyType PropertyType { get; set; }
    public PropertyStatus Status { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public double SquareFeet { get; set; }
    public int YearBuilt { get; set; }
    public bool HasParking { get; set; }
    public bool HasGarden { get; set; }
    public bool HasPool { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public List<string> Images { get; set; } = new();
    public string OwnerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreatePropertyDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Location { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public PropertyType PropertyType { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public double SquareFeet { get; set; }
    public int YearBuilt { get; set; }
    public bool HasParking { get; set; }
    public bool HasGarden { get; set; }
    public bool HasPool { get; set; }
    public List<IFormFile> Images { get; set; }
}

public class UpdatePropertyDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? Location { get; set; }
    public string? City { get; set; }
    public PropertyStatus? Status { get; set; }
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public double? SquareFeet { get; set; }
    public bool? HasParking { get; set; }
    public bool? HasGarden { get; set; }
    public bool? HasPool { get; set; }
}

public class PropertyFilterDto
{
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Location { get; set; }
    public string? City { get; set; }
    public PropertyType? PropertyType { get; set; }
    public PropertyStatus? Status { get; set; }
    public int? MinBedrooms { get; set; }
    public int? MinBathrooms { get; set; }
    public double? MinSquareFeet { get; set; }
    public bool? HasParking { get; set; }
    public bool? HasGarden { get; set; }
    public bool? HasPool { get; set; }
    public string? SortBy { get; set; } // price, created, rating
    public bool SortDescending { get; set; } = false;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
