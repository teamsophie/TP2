using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace PaymentService.Models
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
            : base(options)
        {
        }

        public DbSet<PaiementModel> Paiements { get; set; }
    }
}
