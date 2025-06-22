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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Exemple : configurer une clé primaire personnalisée
            modelBuilder.Entity<Client>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Produit>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Achat>()
                .HasKey(a => a.Id); // très important : EF a besoin d'une clé

            // Tu peux aussi ajouter des contraintes supplémentaires ici :
            modelBuilder.Entity<Client>()
                .Property(c => c.Nom)
                .IsRequired()
                .HasMaxLength(100);
                
            modelBuilder.Entity<Client>().HasData(
                new Client { Id = 1, Nom = "Alice", Age = 30, Ville = "Paris" },
                new Client { Id = 2, Nom = "Bob", Age = 42, Ville = "Lyon" }
            );
        }
    }
}
