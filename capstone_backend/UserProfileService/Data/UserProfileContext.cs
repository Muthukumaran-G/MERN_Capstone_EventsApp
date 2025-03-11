using Microsoft.EntityFrameworkCore;
using UserProfileService.Models;

namespace UserProfileService.Data
{
    public class UserProfileContext : DbContext
    {
        public UserProfileContext(DbContextOptions<UserProfileContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
