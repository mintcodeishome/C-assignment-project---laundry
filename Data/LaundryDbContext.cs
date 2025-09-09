using Microsoft.EntityFrameworkCore;
using Laundry.Models;

namespace Laundry.Data;

public class LaundryDbContext : DbContext
{
    public LaundryDbContext(DbContextOptions<LaundryDbContext> options) : base(options)
    {
    }

    public DbSet<LaundryOrder> LaundryOrders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LaundryOrder>().ToTable("LaundryOrders");
        modelBuilder.Entity<OrderItem>().ToTable("OrderItems");
        
        // Configure relationships
        modelBuilder.Entity<LaundryOrder>()
            .HasMany(o => o.Items)
            .WithOne(i => i.LaundryOrder)
            .HasForeignKey(i => i.LaundryOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}