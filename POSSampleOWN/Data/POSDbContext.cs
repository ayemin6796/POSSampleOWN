using Microsoft.EntityFrameworkCore;
using POSSampleOWN.Models;

namespace POSSampleOWN.Data
{
    public class POSDbContext: DbContext
    {
        public POSDbContext(DbContextOptions<POSDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
