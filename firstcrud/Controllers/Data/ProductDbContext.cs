using Microsoft.EntityFrameworkCore;

namespace firstcrud.Controllers.Data;

public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products =>  Set<Product>();
    public DbSet<ProductDetails> ProductDetails =>  Set<ProductDetails>();
}