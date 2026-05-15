using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.Application.DTOs.Common;
using RealEstateAPI.Application.DTOs.Property;
using RealEstateAPI.Application.Interfaces;
using System.Security.Claims;

namespace RealEstateAPI.API.Controllers;

/// <summary>
/// Properties controller for managing real estate properties
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _propertyService;
    private readonly ILogger<PropertiesController> _logger;

    public PropertiesController(IPropertyService propertyService, ILogger<PropertiesController> logger)
    {
        _propertyService = propertyService;
        _logger = logger;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User ID not found"));
    }

    [Authorize]
    [HttpGet("mine")]
    public async Task<IActionResult> GetMyProperties()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out Guid ownerId))
            return Unauthorized("Invalid user token");

        var result = await _propertyService.GetMyPropertiesAsync(ownerId);

        return Ok(result);
    }

    /// <summary>
    /// Get all properties with filtering, sorting, and pagination
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<PropertyDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] PropertyFilterDto filter)
    {
        try
        {
            var result = await _propertyService.GetAllAsync(filter);
            return Ok(ApiResponse<PagedResponse<PropertyDto>>.SuccessResponse(
                result, "Properties retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving properties");
            return StatusCode(500, ApiResponse<object>.FailureResponse(
                "An error occurred while retrieving properties"));
        }
    }

    /// <summary>
    /// Get property by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PropertyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _propertyService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(ApiResponse<object>.FailureResponse("Property not found"));
            }

            return Ok(ApiResponse<PropertyDto>.SuccessResponse(
                result, "Property retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving property {PropertyId}", id);
            return StatusCode(500, ApiResponse<object>.FailureResponse(
                "An error occurred while retrieving the property"));
        }
    }

    /// <summary>
    /// Create a new property
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PropertyDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromForm] CreatePropertyDto dto)
    {
        try
        {
            var userId = GetUserId();
            var result = await _propertyService.CreateAsync(dto, userId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                ApiResponse<PropertyDto>.SuccessResponse(result, "Property created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating property");
            return StatusCode(500, ApiResponse<object>.FailureResponse(
                "An error occurred while creating the property"));
        }
    }

    /// <summary>
    /// Update an existing property
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePropertyDto dto)
    {
        try
        {
            var userId = GetUserId();
            var success = await _propertyService.UpdateAsync(id, dto, userId);

            if (!success)
            {
                return NotFound(ApiResponse<object>.FailureResponse(
                    "Property not found or you don't have permission to update it"));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Property updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating property {PropertyId}", id);
            return StatusCode(500, ApiResponse<object>.FailureResponse(
                "An error occurred while updating the property"));
        }
    }

    /// <summary>
    /// Delete a property
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var success = await _propertyService.DeleteAsync(id, userId);

            if (!success)
            {
                return NotFound(ApiResponse<object>.FailureResponse(
                    "Property not found or you don't have permission to delete it"));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Property deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting property {PropertyId}", id);
            return StatusCode(500, ApiResponse<object>.FailureResponse(
                "An error occurred while deleting the property"));
        }
    }












}
