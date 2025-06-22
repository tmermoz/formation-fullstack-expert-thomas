using ApiCatalogue.Models;
using ApiCatalogue.Data;
using Microsoft.EntityFrameworkCore;

// <summary>
// InMemory implementation of the IClientRepository interface.  
// This repository uses a static list to simulate a database.
// </summary>
namespace ApiCatalogue.Repositories.InMemory
{
    public class ProduitRepository : IProduitRepository
    {
        private readonly CatalogueDbContext _context;

        public ProduitRepository(CatalogueDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all produits.  
        /// /// </summary>
        /// <returns>A list of produits.</returns>
        public async Task<IEnumerable<Produit>> GetAllProduitsAsync()
        {
            return await _context.Produits.ToListAsync();
        }   
        
        public Task AddProduitAsync(Produit produit)
        {
            _context.Produits.Add(produit);
            return Task.CompletedTask;
        }

        public async Task DeleteProduitAsync(int id)
        {
            var produit = await _context.Produits.FirstOrDefaultAsync(p => p.Id == id);
            if (produit != null)
            {
                _context.Produits.Remove(produit);
                await _context.SaveChangesAsync();
            }
        }
    }
}

