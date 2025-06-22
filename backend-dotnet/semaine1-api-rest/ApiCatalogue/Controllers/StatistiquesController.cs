using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiCatalogue.Data;
using ApiCatalogue.Dtos;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatistiquesController : ControllerBase
    {
        private readonly CatalogueDbContext _context;
        private readonly ILogger<StatistiquesController> _logger;

        public StatistiquesController(CatalogueDbContext context, ILogger<StatistiquesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("produits-par-ville")]
        public async Task<IActionResult> GetStatsProduitsParVille()
        {
            var stats = await _context.Achats
                .Include(a => a.Client)
                .Include(a => a.Produit)
                .GroupBy(a => a.Client.Ville)
                .Select(group => new TopClientDto
                {
                    NomClient = group.Key,
                    NombreAchats = group.Count(),
                    TotalDepense = group.Sum(a => a.Produit.Prix)
                })
                .OrderByDescending(dto => dto.TotalDepense)
                .ToListAsync();

            _logger.LogInformation("Statistiques des produits par ville calculées avec succès.");

            return Ok(stats);
        }
    }
}
