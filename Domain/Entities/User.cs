namespace RealEstateAPI.Domain.Entities;

/// <summary>
/// User entity representing system users (Admin, Regular Users)
/// </summary>
public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // Admin, User
    public bool IsActive { get; set; } = true;
    
    // Navigation Properties
    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public virtual ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();
}
