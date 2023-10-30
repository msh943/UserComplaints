using Core.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
    
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {
            
        }
        
        public DbSet<Demand> demands { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
    }
}
