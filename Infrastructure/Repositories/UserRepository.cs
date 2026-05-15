using Microsoft.EntityFrameworkCore;
using RealEstateAPI.Domain.Entities;
using RealEstateAPI.Infrastructure.Data;
using RealEstateAPI.Infrastructure.Repositories.Interfaces;

namespace RealEstateAPI.Infrastructure.Repositories;

/// <summary>
/// User repository with custom user-specific queries
/// </summary>
public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }
}
