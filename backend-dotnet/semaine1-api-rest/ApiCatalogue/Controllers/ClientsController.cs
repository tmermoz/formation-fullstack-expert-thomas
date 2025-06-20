using Microsoft.AspNetCore.Mvc;
using ApiCatalogue.Models;
using System.Collections.Generic;
using System.Linq;

namespace ApiCatalogue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ILogger<ClientsController> _logger;

        private static readonly List<Client> Clients = new()
        {
            new Client { Nom = "Alice", Age = 30, Ville = "Paris" },
            new Client { Nom = "Bob", Age = 42, Ville = "Lyon" },
            new Client { Nom = "Claire", Age = 25, Ville = "Paris" },
            new Client { Nom = "David", Age = 35, Ville = "Toulouse" },
        };

        public ClientsController(ILogger<ClientsController> logger)
        {
            _logger = logger;
        }        

        [HttpGet("search")]
        public ActionResult<IEnumerable<string>> GetClients(
            [FromQuery] string? ville,
            [FromQuery] int? age)
        {
            if (string.IsNullOrWhiteSpace(ville) || !age.HasValue)
            {
                _logger.LogWarning("Paramètres invalides : ville='{Ville}', age='{Age}'", ville, age);
                return BadRequest("Veuillez fournir une ville et un âge maximum.");
            }

            _logger.LogInformation($"Recherche de clients pour ville = {ville} et age < {age}");

            var nomsClientsFiltres = Clients
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