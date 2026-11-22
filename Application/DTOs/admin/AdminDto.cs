using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAPI.Application.DTOs.admin
{
    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }

        public int TotalProperties { get; set; }

        public int TotalBrokers { get; set; }

        public int ActiveProperties { get; set; }

        public int SoldProperties { get; set; }
    }


    public class CreateBrokerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
