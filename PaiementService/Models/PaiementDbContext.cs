using Microsoft.EntityFrameworkCore;
using PaiementService.Models;

namespace PaiementService.Data
{
    public class PaiementDbContext : DbContext
    {
        public PaiementDbContext(DbContextOptions<PaiementDbContext> options)
            : base(options)
        {
        }

        public DbSet<Paiement> Paiements { get; set; }
    }
}
