using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealEstateAPI.Application.DTOs.admin;
using RealEstateAPI.Application.Interfaces;
using RealEstateAPI.Domain.Entities;
using RealEstateAPI.Domain.Enums;
using RealEstateAPI.Infrastructure.Data;
using RealEstateAPI.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAPI.Infrastructure.Services
{
    public class AdminService : IAdminService
    {

        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
             IUserRepository userRepository,
            ApplicationDbContext context,
            ILogger<AdminService> logger)
        {
            _userRepository = userRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<AdminDashboardDto> GetDashboardAsync()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();

                var totalProperties = await _context.Properties.CountAsync();

                var totalBrokers = await _context.Users
                    .CountAsync(x => x.Role == UserRole.Broker);

                var activeProperties = await _context.Properties
                    .CountAsync(x => x.Status == PropertyStatus.Available);

                var soldProperties = await _context.Properties
                    .CountAsync(x => x.Status == PropertyStatus.Sold);

                return new AdminDashboardDto
                {
                    TotalUsers = totalUsers,
                    TotalProperties = totalProperties,
                    TotalBrokers = totalBrokers,
                    ActiveProperties = activeProperties,
                    SoldProperties = soldProperties
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading dashboard");

                throw;
            }
        }



        public async Task CreateBrokerAsync(CreateBrokerDto dto)
        {
            var broker = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.Broker,
                IsActive = true
            };

            await _userRepository.AddAsync(broker);
        }

    }
}
