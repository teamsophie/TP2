using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CartService.Models
{
    public class CartDbContext : DbContext
    {
        public CartDbContext(DbContextOptions<CartDbContext> options) : base(options) { }

        public DbSet<CartItem> CartItems { get; set; }
    }
}
