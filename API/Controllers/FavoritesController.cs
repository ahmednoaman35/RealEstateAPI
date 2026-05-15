using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.Application.DTOs.Common;
using RealEstateAPI.Application.DTOs.Property;
using RealEstateAPI.Application.Interfaces;
using System.Security.Claims;

namespace RealEstateAPI.API.Controllers;

/// <summary>
/// Favorites controller for managing user's favorite properties
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;
    private readonly ILogger<FavoritesController> _logger;

    public FavoritesController(IFavoriteService favoriteService, ILogger<FavoritesController> logger)
    {
        _favoriteService = favoriteService;
        _logger = logger;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
    }

    /// <summary>
    /// Get user's favorite properties
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<PropertyDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserFavorites([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var userId = GetUserId();
            var result = await _favoriteService.GetUserFavoritesAsync(userId, pageNumber, pageSize);
            
            return Ok(ApiResponse<PagedResponse<PropertyDto>>.SuccessResponse(
                result, "Favorites retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving favorites");
            return StatusCode(500, ApiResponse<object>.FailureResponse(
                "An error occurred while retrieving favorites"));
        }
    }

    /// <summary>
    /// Add property to favorites
    /// </summary>
    [HttpPost("{propertyId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddToFavorites(Guid propertyId)
    {
        try
        {
            var userId = GetUserId();
            var success = await _favoriteService.AddToFavoritesAsync(propertyId, userId);

            if (!success)
            {
                return BadRequest(ApiResponse<object>.FailureResponse(
                    "Property is already in favorites"));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Property added to favorites"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding to favorites");
            return StatusCode(500, ApiResponse<object>.FailureResponse(
                "An error occurred while adding to favorites"));
        }
    }

    /// <summary>
    /// Remove property from favorites
    /// </summary>
    [HttpDelete("{propertyId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFromFavorites(Guid propertyId)
    {
        try
        {
            var userId = GetUserId();
            var success = await _favoriteService.RemoveFromFavoritesAsync(propertyId, userId);

            if (!success)
            {
                return NotFound(ApiResponse<object>.FailureResponse(
                    "Property not found in favorites"));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Property removed from favorites"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing from favorites");
            return StatusCode(500, ApiResponse<object>.FailureResponse(
                "An error occurred while removing from favorites"));
        }
    }

    /// <summary>
    /// Check if property is in user's favorites
    /// </summary>
    [HttpGet("check/{propertyId}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> IsFavorite(Guid propertyId)
    {
        try
        {
            var userId = GetUserId();
            var isFavorite = await _favoriteService.IsFavoriteAsync(propertyId, userId);
            
            return Ok(ApiResponse<bool>.SuccessResponse(isFavorite, "Favorite status retrieved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking favorite status");
            return StatusCode(500, ApiResponse<object>.FailureResponse(
                "An error occurred while checking favorite status"));
        }
    }
}
