using RealEstateAPI.Domain.Enums;

namespace RealEstateAPI.Domain.Entities;

/// <summary>
/// Inquiry entity for property booking/inquiry requests
/// </summary>
public class Inquiry : BaseEntity
{
    public string Message { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    
    public DateTime PreferredContactDate { get; set; }
    public InquiryStatus Status { get; set; } = InquiryStatus.Pending;
    
    public Guid PropertyId { get; set; }
    public Guid UserId { get; set; }
    
    public string? AdminResponse { get; set; }
    public DateTime? RespondedAt { get; set; }
    
    // Navigation Properties
    public virtual Property Property { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
