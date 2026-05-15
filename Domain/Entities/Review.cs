namespace RealEstateAPI.Domain.Entities;

/// <summary>
/// Review entity for property ratings and comments
/// </summary>
public class Review : BaseEntity
{
    public int Rating { get; set; } // 1-5 stars
    public string Comment { get; set; } = string.Empty;
    
    public Guid PropertyId { get; set; }
    public Guid UserId { get; set; }
    
    // Navigation Properties
    public virtual Property Property { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
