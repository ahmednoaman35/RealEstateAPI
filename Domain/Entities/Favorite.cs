namespace RealEstateAPI.Domain.Entities;

/// <summary>
/// Favorite entity representing user's favorite properties (Many-to-Many)
/// </summary>
public class Favorite : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid PropertyId { get; set; }
    
    // Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual Property Property { get; set; } = null!;
}
