using RealEstateAPI.Domain.Enums;

namespace RealEstateAPI.Application.DTOs.Inquiry;

public class InquiryDto
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public DateTime PreferredContactDate { get; set; }
    public InquiryStatus Status { get; set; }
    public Guid PropertyId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? AdminResponse { get; set; }
    public DateTime? RespondedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateInquiryDto
{
    public Guid PropertyId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public DateTime PreferredContactDate { get; set; }
}

public class UpdateInquiryStatusDto
{
    public InquiryStatus Status { get; set; }
    public string? AdminResponse { get; set; }
}
