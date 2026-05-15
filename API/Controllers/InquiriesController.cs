using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.Application.DTOs.Common;
using RealEstateAPI.Application.DTOs.Inquiry;
using RealEstateAPI.Application.Interfaces;
using System.Security.Claims;

namespace RealEstateAPI.API.Controllers;

/// <summary>
/// Inquiries controller for property booking/inquiry requests
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InquiriesController : ControllerBase
{
    private readonly IInquiryService _inquiryService;
    private readonly ILogger<InquiriesController> _logger;

    public InquiriesController(IInquiryService inquiryService, ILogger<InquiriesController> logger)
    {
        _inquiryService = inquiryService;
        _logger = logger;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
    }

    private bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }

    /// <summary>
    /// Get inquiry by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<InquiryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _inquiryService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(ApiResponse<object>.FailureResponse("Inquiry not found"));
            }

            return Ok(ApiResponse<InquiryDto>.SuccessResponse(
                result, "Inquiry retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inquiry {InquiryId}", id);
            return StatusCode(500, ApiResponse<object>.FailureResponse(
                "An error occurred while retrieving the inquiry"));
        }
    }

    /// <summary>
    /// Get user's inquiries
    /// </summary>
    [HttpGet("my-inquiries")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<InquiryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyInquiries(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var userId = GetUserId();
            var result = await _inquiryService.GetUserInquiriesAsync(userId, pageNumber, pageSize);
            
            return Ok(ApiResponse<PagedResponse<InquiryDto>>.SuccessResponse(
                result, "Inquiries retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user inquiries");
            return StatusCode(500, ApiResponse<object>.FailureResponse(
                "An error occurred while retrieving inquiries"));
        }
    }

    /// <summary>
    /// Get all inquiries (Admin only)
    /// </summary>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<InquiryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _inquiryService.GetAllInquiriesAsync(pageNumber, pageSize);
            
            return Ok(ApiResponse<PagedResponse<InquiryDto>>.SuccessResponse(
                result, "All inquiries retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all inquiries");
            return StatusCode(500, ApiResponse<object>.FailureResponse(
                "An error occurred while retrieving inquiries"));
        }
    }

    /// <summary>
    /// Create a new inquiry
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<InquiryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateInquiryDto dto)
    {
        try
        {
            var userId = GetUserId();
            var result = await _inquiryService.CreateAsync(dto, userId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                ApiResponse<InquiryDto>.SuccessResponse(result, "Inquiry created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating inquiry");
            return StatusCode(500, ApiResponse<object>.FailureResponse(
                "An error occurred while creating the inquiry"));
        }
    }

    /// <summary>
    /// Update inquiry status (Admin only)
    /// </summary>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateInquiryStatusDto dto)
    {
        try
        {
            var success = await _inquiryService.UpdateStatusAsync(id, dto);

            if (!success)
            {
                return NotFound(ApiResponse<object>.FailureResponse("Inquiry not found"));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Inquiry status updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating inquiry status {InquiryId}", id);
            return StatusCode(500, ApiResponse<object>.FailureResponse(
                "An error occurred while updating the inquiry status"));
        }
    }

    /// <summary>
    /// Delete an inquiry
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var success = await _inquiryService.DeleteAsync(id, userId);

            if (!success)
            {
                return NotFound(ApiResponse<object>.FailureResponse(
                    "Inquiry not found or you don't have permission to delete it"));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Inquiry deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting inquiry {InquiryId}", id);
            return StatusCode(500, ApiResponse<object>.FailureResponse(
                "An error occurred while deleting the inquiry"));
        }
    }
}
