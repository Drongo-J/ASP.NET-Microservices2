using Authentication.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Authentication.API.Data
{
    public class UsersDbContext : DbContext
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options)
          : base(options)
        {
        }
        public DbSet<User> Users { get; set; }

    }
}
