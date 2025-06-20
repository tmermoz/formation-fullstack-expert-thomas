using Microsoft.AspNetCore.Mvc;
using ApiCatalogue.Models;
using System.Collections.Generic;
using System.Linq;

namespace ApiCatalogue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatistiquesController : ControllerBase
    {
        private readonly ILogger<StatistiquesController> _logger;

        private static readonly List<Achat> Achats = new()
        {
            new Achat { NomClient = "Alice", NomProduit = "Ordinateur portable" },
            new Achat { NomClient = "Bob", NomProduit = "Smartphone" },
            new Achat { NomClient = "Claire", NomProduit = "Ordinateur portable" },
            new Achat { NomClient = "David", NomProduit = "Ecouteurs bluetooth" },
            new Achat { NomClient = "Alice", NomProduit = "Ecouteurs bluetooth" },
        };

        private static readonly List<Produit> Produits = new()
        {
            new Produit {Id = 1, Nom = "Ordinateur portable", Prix = 999.99m, Categorie = "Informatique"},
            new Produit {Id = 2, Nom = "Smartphone", Prix = 749.99m, Categorie = "Téléphonie"},
            new Produit {Id = 3, Nom = "Ecouteurs bluetooth", Prix = 129.00m, Categorie = "Audio"}
        };

        private static readonly List<Client> Clients = new()
        {
            new Client { Nom = "Alice", Age = 30, Ville = "Paris" },
            new Client { Nom = "Bob", Age = 42, Ville = "Lyon" },
            new Client { Nom = "Claire", Age = 25, Ville = "Paris" },
            new Client { Nom = "David", Age = 35, Ville = "Toulouse" },
        };

        public StatistiquesController(ILogger<StatistiquesController> logger)
        {
            _logger = logger;
        }

        [HttpGet("produits-par-ville")]
        public IActionResult GetStatsProduitsParVille()
        {
            var stats = Achats
                .Join(Clients,
                    achat => achat.NomClient,
                    client => client.Nom,
                    (achat, client) => new { achat.NomProduit, client.Ville })
                .Join(Produits,
                    ac => ac.NomProduit,
                    produit => produit.Nom,
                    (ac, produit) => new { ac.Ville, produit.Prix })
                .GroupBy(x => x.Ville)
                .Select(group => new
                {
                    Ville = group.Key,
                    NombreProduits = group.Count(),
                    Total = group.Sum(x => x.Prix),
                    Max = group.Max(x => x.Prix),
                    Min = group.Min(x => x.Prix)
                });

            _logger.LogInformation("Statistiques des produits par ville calculées avec succès.");

            return Ok(stats);
        }
    }
}