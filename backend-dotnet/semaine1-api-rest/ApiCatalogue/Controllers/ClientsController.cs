using Microsoft.AspNetCore.Mvc;
using ApiCatalogue.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using ApiCatalogue.Repositories;
using System.Threading.Tasks;

namespace ApiCatalogue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ILogger<ClientsController> _logger;

        private readonly IClientRepository _clientRepo;

        public ClientsController(ILogger<ClientsController> logger, IClientRepository clientRepo)
        {
            _logger = logger;
            _clientRepo = clientRepo;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<string>>> GetClients(
            [FromQuery] string? ville,
            [FromQuery] int? age)
        {
            if (string.IsNullOrWhiteSpace(ville) || !age.HasValue)
            {
                _logger.LogWarning("Paramètres invalides : ville='{Ville}', age='{Age}'", ville, age);
                return BadRequest("Veuillez fournir une ville et un âge maximum.");
            }

            _logger.LogInformation($"Recherche de clients pour ville = {ville} et age < {age}");

            var clients = await _clientRepo.GetAllClientsAsync();
            if (clients == null || !clients.Any())
            {
                _logger.LogInformation("Aucun client trouvé.");
                return NotFound("Aucun client trouvé.");
            }   

            var nomsClientsFiltres = clients
                                    .Where(c =>
                                        c.Ville != null &&
                                        c.Ville.Equals(ville, StringComparison.OrdinalIgnoreCase) &&
                                        c.Age < age.Value)
                                    .Select(c => c.Nom)
                                    .ToList();

            _logger.LogInformation("Résultats : {Noms}", string.Join(", ", nomsClientsFiltres));

            return Ok(nomsClientsFiltres);
        }
    }
}