using Microsoft.AspNetCore.Mvc;
using ApiCatalogue.Models;
using ApiCatalogue.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ApiCatalogue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysesController : ControllerBase
    {
        private readonly ILogger<AnalysesController> _logger;

        private static readonly List<Achat> Achats = new()
        {
            new Achat { NomClient = "Alice", NomProduit = "Ordinateur portable" },
            new Achat { NomClient = "Alice", NomProduit = "Ecouteurs bluetooth" },
            new Achat { NomClient = "Bob", NomProduit = "Smartphone" },
            new Achat { NomClient = "Claire", NomProduit = "Ecouteurs bluetooth" },
            new Achat { NomClient = "David", NomProduit = "Ordinateur portable" },
        };

        private static readonly List<Produit> Produits = new()
        {
            new Produit { Nom = "Ordinateur portable", Prix = 1000 },
            new Produit { Nom = "Smartphone", Prix = 800 },
            new Produit { Nom = "Ecouteurs bluetooth", Prix = 100 }
        };

        public AnalysesController(ILogger<AnalysesController> logger)
        {
            _logger = logger;
        }

        [HttpGet("meilleurs-clients")]
        public ActionResult<IEnumerable<TopClientDto>> GetTopClientsBySpending([FromQuery] int n = 2)
        {
            if (n <= 0)
            {
                _logger.LogWarning("Nombre de clients demandé invalide : {n}", n);
                return BadRequest("Le nombre de clients doit être supérieur à zéro.");
            }

            var stats = Achats
                .Join(Produits,
                      achat => achat.NomProduit,
                      produit => produit.Nom,
                      (achat, produit) => new { achat.NomClient, produit.Prix })
                .GroupBy(x => x.NomClient)
                .Select(g => new TopClientDto
                {
                    NomClient = g.Key,
                    TotalDepense = g.Sum(x => x.Prix),
                    NombreAchats = g.Count()
                })
                .OrderByDescending(x => x.TotalDepense)
                .Take(n)
                .ToList();

            _logger.LogInformation("Top {Count} clients récupérés.", stats.Count);

            return Ok(stats);        
        }
    }
}