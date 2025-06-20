using Microsoft.AspNetCore.Mvc;
using ApiCatalogue.Models;
using ApiCatalogue.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ApiCatalogue.Controllers;
using Microsoft.Extensions.Logging;
using ApiCatalogue.Repositories;

namespace ApiCatalogue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysesController : ControllerBase
    {
        private readonly ILogger<AnalysesController> _logger;
        private readonly IClientRepository _clientRepo;
        private readonly IProduitRepository _produitRepo;
        private readonly IAchatRepository _achatRepo;

        public AnalysesController(ILogger<AnalysesController> logger,
                        IClientRepository clientRepo,
                        IProduitRepository produitRepo,
                        IAchatRepository achatRepo)
        {
            _logger = logger;
            _clientRepo = clientRepo;
            _produitRepo = produitRepo;
            _achatRepo = achatRepo;
        }

        [HttpGet("meilleurs-clients")]
        public async Task<ActionResult<IEnumerable<TopClientDto>>> GetTopClientsBySpending([FromQuery] int n = 2)
        {
            if (n <= 0)
            {
                _logger.LogWarning("Nombre de clients demandé invalide : {n}", n);
                return BadRequest("Le nombre de clients doit être supérieur à zéro.");
            }

            var achats = await _achatRepo.GetAllAchatsAsync();
            var produits = await _produitRepo.GetAllProduitsAsync();

            if (!produits.Any())
            {
                _logger.LogWarning("Aucun produit trouvé.");
                return NotFound("Aucun produit enregistré.");
            }
            if (!achats.Any())
            {
                _logger.LogWarning("Aucun achat trouvé.");
                return NotFound("Aucun achat enregistré.");
            }   

            var stats = achats
                .Join(produits,
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
            _logger.LogDebug("Calcul effectué sur {NombreAchats} achats.", achats.Count());

            return Ok(stats);        
        }
    }
}