using ApiCatalogue.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiCatalogue.Repositories
{
    /// <summary>
    /// Interface for the Produit repository.
    /// </summary>
    public interface IProduitRepository
    {
        Task AddProduitAsync(Produit produit);
        Task DeleteProduitAsync(int id);

        /// <summary>
        /// Gets all produits.
        /// </summary>
        /// <returns>A list of produits.</returns>
        Task<IEnumerable<Produit>> GetAllProduitsAsync();
    }
}