using RealEstateAPI.Domain.Entities;
using RealEstateAPI.Domain.Enums;

public class User : BaseEntity
{
    // الكود الموجود
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsActive { get; set; } = true;

    // ⭐ أضف السطرين دول بس
    public string? WhatsAppNumber { get; set; }
    public bool EnableWhatsApp { get; set; } = true;

    // Navigation Properties (زي ما هي)
    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public virtual ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();
}