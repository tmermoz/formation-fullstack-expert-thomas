using Microsoft.EntityFrameworkCore;
using ApiCatalogue.Models;

namespace ApiCatalogue.Data
{
    public class CatalogueDbContext : DbContext
    {
        public CatalogueDbContext(DbContextOptions<CatalogueDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Produit> Produits { get; set; }
        public DbSet<Achat> Achats { get; set; }
    }
}
