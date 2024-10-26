using Microsoft.EntityFrameworkCore;
using Onyx.Services.Products.Domain.Resources;

namespace Onyx.Services.Products.Store;

public class ProductsContext : DbContext
{
    public DbSet<Product> Products { get; set; } = default!;
    
    public ProductsContext(DbContextOptions<ProductsContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("products");
        
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable(name: "Products");
            entity.HasKey(e => e.ProductId);

            entity.Property(e => e.ProductId);
            entity.Property(e => e.Description)
                .IsFixedLength()
                .HasMaxLength(500);
            entity.Property(e => e.Name)
                .IsFixedLength()
                .HasMaxLength(100);
            entity.Property(e => e.PriceInMinorUnits);
            entity.Property(e => e.Colour)
                .IsFixedLength()
                .HasMaxLength(50);
        });
    }
}