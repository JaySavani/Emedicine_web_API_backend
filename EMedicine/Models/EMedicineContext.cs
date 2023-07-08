using Microsoft.EntityFrameworkCore;

namespace EMedicine.Models
{
    public class EMedicineContext : DbContext
    {
        internal object orders;

        public EMedicineContext(DbContextOptions<EMedicineContext>options) : base (options)
        {

        }
        public DbSet<Users> Users { get; set; } = null!;

        public DbSet<Loginuser> Loginuser { get; set; } = null!;

        public DbSet<Medicines> Medicines { get; set; } = null!;
        public DbSet<Order> Order { get; set; } = null!;


    }
}
