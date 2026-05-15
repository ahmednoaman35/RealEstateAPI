namespace RealEstateAPI.Domain.Enums;

/// <summary>
/// Inquiry processing status
/// </summary>
public enum InquiryStatus
{
    Pending = 1,
    InProgress = 2,
    Responded = 3,
    Closed = 4,
    Cancelled = 5
}
