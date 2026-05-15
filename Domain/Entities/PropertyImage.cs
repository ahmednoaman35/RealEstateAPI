namespace RealEstateAPI.Domain.Entities;

/// <summary>
/// Property image entity for storing multiple images per property
/// </summary>
public class PropertyImage : BaseEntity
{
    public string ImageUrl { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = false;
    public int DisplayOrder { get; set; }
    
    public Guid PropertyId { get; set; }
    
    // Navigation Properties
    public virtual Property Property { get; set; } = null!;
}
