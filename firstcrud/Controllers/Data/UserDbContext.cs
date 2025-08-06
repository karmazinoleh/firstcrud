using Microsoft.EntityFrameworkCore;

namespace firstcrud.Controllers.Data;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<User> Users =>  Set<User>();
}