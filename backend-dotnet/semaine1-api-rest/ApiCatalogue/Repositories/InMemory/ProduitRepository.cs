// InMemory/ClientRepository.cs
using System.Collections.Generic;
using ApiCatalogue.Models;
using ApiCatalogue.Repositories;
using System.Linq;
using System.Threading.Tasks;

// <summary>
// InMemory implementation of the IClientRepository interface.  
// This repository uses a static list to simulate a database.
// </summary>
namespace ApiCatalogue.Repositories.InMemory
{
    public class ProduitRepository : IProduitRepository
    {
        private static readonly List<Produit> Produits = new()
        {
            new Produit {Id = 1, Nom = "Ordinateur portable", Prix = 999.99m, Categorie = "Informatique"},
            new Produit {Id = 2, Nom = "Smartphone", Prix = 749.99m, Categorie = "Téléphonie"},
            new Produit {Id = 3, Nom = "Ecouteurs bluetooth", Prix = 129.00m, Categorie = "Audio"}
        };

        /// <summary>
        /// Gets all produits.  
        /// /// </summary>
        /// <returns>A list of produits.</returns>
        public Task<IEnumerable<Produit>> GetAllProduitsAsync()
        {
            return Task.FromResult(GetAllProduits());
        }   
        
        public IEnumerable<Produit> GetAllProduits() => Produits;

        public Task AddProduitAsync(Produit produit)
        {
            Produits.Add(produit);
            return Task.CompletedTask;
        }

        public Task DeleteProduitAsync(int id)
        {
            Produits.RemoveAll(p => p.Id == id);
            return Task.CompletedTask;
        }
    }
}

