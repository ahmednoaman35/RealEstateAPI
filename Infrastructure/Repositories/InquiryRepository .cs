using Microsoft.EntityFrameworkCore;
using RealEstateAPI.Domain.Entities;
using RealEstateAPI.Infrastructure.Data;
using RealEstateAPI.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAPI.Infrastructure.Repositories
{
    public class InquiryRepository : BaseRepository<Inquiry>, IInquiryRepository
    {
        public InquiryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Inquiry?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _dbSet
                 .Include(i => i.Property)
                .ThenInclude(p => p.Owner)
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.Id == id);
        }



    }
}
