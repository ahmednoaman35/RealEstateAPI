using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.Application.DTOs.admin;
using RealEstateAPI.Application.Interfaces;
using RealEstateAPI.Domain.Enums;

namespace RealEstateAPI.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class Admin : ControllerBase
    {
        private readonly IAdminService _adminService;

        public Admin(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _adminService.GetDashboardAsync();

            return Ok(result);
        }



      
        [HttpPost("create-broker")]
        public async Task<IActionResult> CreateBroker(CreateBrokerDto dto)
        {
            await _adminService.CreateBrokerAsync(dto);

            return Ok("Broker created successfully");
        }

    }
}

