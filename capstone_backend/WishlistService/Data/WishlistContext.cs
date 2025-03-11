using Microsoft.EntityFrameworkCore;
using WishlistService.Models;

namespace WishlistService.Data
{
    public class WishlistContext : DbContext
    {
        public WishlistContext(DbContextOptions<WishlistContext> options) : base(options) { }

        public DbSet<WishlistItem> WishlistItems { get; set; }
    }
}