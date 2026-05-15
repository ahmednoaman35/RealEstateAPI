using RealEstateAPI.Domain.Enums;

namespace RealEstateAPI.Domain.Entities;

/// <summary>
/// Property entity representing real estate listings
/// </summary>
public class Property : BaseEntity
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
    public PropertyStatus Status { get; set; } = PropertyStatus.Available;
    
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public double SquareFeet { get; set; }
    public int YearBuilt { get; set; }
    
    public bool HasParking { get; set; }
    public bool HasGarden { get; set; }
    public bool HasPool { get; set; }
    
    public Guid OwnerId { get; set; }
    
    // Calculated Property
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    
    // Navigation Properties
    public virtual User Owner { get; set; } = null!;
    public virtual ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public virtual ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();
}
