using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.Application.DTOs.Common;
using RealEstateAPI.Application.DTOs.Review;
using RealEstateAPI.Application.Interfaces;
using System.Security.Claims;

namespace RealEstateAPI.API.Controllers;

/// <summary>
/// Reviews controller for property reviews and ratings
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
    {
        _reviewService = reviewService;
        _logger = logger;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
    }

    /// <summary>
    /// Get review by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<ReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _reviewService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(ApiResponse<object>.FailureResponse("Review not found"));
            }

            return Ok(ApiResponse<ReviewDto>.SuccessResponse(
                result, "Review retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving review {ReviewId}", id);
            return StatusCode(500, ApiResponse<object>.FailureResponse(
                "An error occurred while retrieving the review"));
        }
    }

    /// <summary>
    /// Get reviews for a specific property
    /// </summary>
    [HttpGet("property/{propertyId}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<ReviewDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByPropertyId(
        Guid propertyId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _reviewService.GetByPropertyIdAsync(propertyId, pageNumber, pageSize);
            
            return Ok(ApiResponse<PagedResponse<ReviewDto>>.SuccessResponse(
                result, "Reviews retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reviews for property {PropertyId}", propertyId);
            return StatusCode(500, ApiResponse<object>.FailureResponse(
                "An error occurred while retrieving reviews"));
        }
    }

    /// <summary>
    /// Create a new review for a property
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ReviewDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
    {
        try
        {
            var userId = GetUserId();
            var result = await _reviewService.CreateAsync(dto, userId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                ApiResponse<ReviewDto>.SuccessResponse(result, "Review created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid review creation attempt");
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating review");
            return StatusCode(500, ApiResponse<object>.FailureResponse(
                "An error occurred while creating the review"));
        }
    }

    /// <summary>
    /// Update an existing review
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReviewDto dto)
    {
        try
        {
            var userId = GetUserId();
            var success = await _reviewService.UpdateAsync(id, dto, userId);

            if (!success)
            {
                return NotFound(ApiResponse<object>.FailureResponse(
                    "Review not found or you don't have permission to update it"));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Review updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating review {ReviewId}", id);
            return StatusCode(500, ApiResponse<object>.FailureResponse(
                "An error occurred while updating the review"));
        }
    }

    /// <summary>
    /// Delete a review
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var success = await _reviewService.DeleteAsync(id, userId);

            if (!success)
            {
                return NotFound(ApiResponse<object>.FailureResponse(
                    "Review not found or you don't have permission to delete it"));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Review deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting review {ReviewId}", id);
            return StatusCode(500, ApiResponse<object>.FailureResponse(
                "An error occurred while deleting the review"));
        }
    }
}
