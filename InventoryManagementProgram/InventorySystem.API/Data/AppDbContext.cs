using Microsoft.EntityFrameworkCore;
using InventorySystem.API.Models;

namespace InventorySystem.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Scale> Scales { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<WeightReading> WeightReadings { get; set; }
        public DbSet<ChangeRequest> ChangeRequests { get; set; }
    }
}