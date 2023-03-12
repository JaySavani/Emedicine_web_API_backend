using Microsoft.EntityFrameworkCore;

namespace EMedicine.Models
{
    public class EMedicineContext : DbContext
    {
        public EMedicineContext(DbContextOptions<EMedicineContext>options) : base (options)
        {

        }

        public DbSet<Users> Users { get; set; } = null!;
        public DbSet<Medicines> Medicines { get; set; } = null!;

    }
}
