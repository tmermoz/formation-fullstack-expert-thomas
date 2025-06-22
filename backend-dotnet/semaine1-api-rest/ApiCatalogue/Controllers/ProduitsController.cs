using Microsoft.AspNetCore.Mvc;
using ApiCatalogue.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.Logging;
using System;   
using System.Threading.Tasks;
using ApiCatalogue.Repositories.InMemory;
using ApiCatalogue.Repositories;

namespace ApiCatalogue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProduitsController : ControllerBase
    {
        private readonly ILogger<ProduitsController> _logger;
        private readonly IProduitRepository _produitRepository; 

        private static readonly List<Categorie> Categories = new()
        {
            new Categorie { Nom = "Informatique", Description = "Ordinateurs et périphériques" },
            new Categorie { Nom = "Téléphonie", Description = "Smartphones et accessoires" },
            new Categorie { Nom = "Audio", Description = "Équipements audio" }
        };

        public ProduitsController(ILogger<ProduitsController> logger, IProduitRepository produitRepository)
        {
            _produitRepository = produitRepository;
            _logger = logger;
        }

        #region ancien code 
        // [HttpGet]
        // public ActionResult<IEnumerable<Produit>> GetProduits()
        // {
        //     return Ok(Produits);
        // }

        // [HttpPost]
        // public ActionResult<Produit> AjouterProduit([FromBody] Produit nouveauProduit)
        // {
        //     if (Produits.Any(p => p.Id == nouveauProduit.Id))
        //         return Conflict($"Un produit avec l'Id {nouveauProduit.Id} existe déjà.");

        //     Produits.Add(nouveauProduit);
        //     return CreatedAtAction(nameof(GetProduits), new { id = nouveauProduit.Id }, nouveauProduit);
        // }

        // [HttpGet]
        // public ActionResult<IEnumerable<Produit>> GetProduits()
        // {
        //     var produits = new List<Produit>
        //     {
        //         new Produit { Id = 1, Nom = "PC Portable", Prix = 999.99m, Categorie = "Informatique" },
        //         new Produit { Id = 2, Nom = "Smartphone", Prix = 749.99m, Categorie = "Téléphonie" },
        //         new Produit { Id = 3, Nom = "Écouteurs", Prix = 129.00m, Categorie = "Audio" }
        //     };

        //     return Ok(produits);
        // }
        #endregion ancien code

        /// <summary>
        /// Récupère la liste des produits, éventuellement filtrés par catégorie
        /// </summary>
        /// <param name="categorie">Catégorie du produit à filtrer (optionnelle)</param>
        /// <returns>Liste des produits (filtrée ou complète)</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Produit>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Produit>>> GetProduits([FromQuery] string? categorie)
        {
            if (string.IsNullOrEmpty(categorie))
                return BadRequest("Catégorie manquante.");

            //Console.WriteLine($"categorie : {categorie}");
            _logger.LogInformation("Récupération des produits avec catégorie : {Categorie}", categorie);

            var produits = await _produitRepository.GetAllProduitsAsync();
            if (produits == null || !produits.Any())
            {
                _logger.LogWarning("Aucun produit trouvé.");
                return NotFound("Aucun produit trouvé.");
            }

            // Récupère tous les produits via le repository
            var produitsFiltres = produits
                    .Where(p => p.Categorie.Equals(categorie, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            return Ok(produitsFiltres);
        }

        /// <summary>
        /// Retourne le produit selon l'id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Produit), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Produit>> GetProduitByIdAsync(int id)
        {
            _logger.LogInformation("Recherche du produit avec l'Id : {Id}", id);

            var produits = await _produitRepository.GetAllProduitsAsync();
            if (produits == null || !produits.Any())
            {
                _logger.LogWarning("Aucun produit trouvé.");
                return NotFound("Aucun produit trouvé.");
            }
            
            var produit = produits.FirstOrDefault(p => p.Id == id);
            if (produit == null)
                return NotFound($"Aucun produit avec l'Id {id}");

            return Ok(produit);
        }

        /// <summary>
        /// Crée un nouveau produit
        /// </summary>
        /// <param name="produit">Produit à ajouter à la liste</param>
        /// <returns>Produit créé avec son identifiant</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Produit), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Produit>> AddProduitAsync([FromBody] Produit produit)
        {
            if (produit == null || string.IsNullOrWhiteSpace(produit.Nom))
                return BadRequest("Produit invalide");
            
            _logger.LogInformation("Ajout d'un nouveau produit : {Nom}", produit.Nom);

            var produits = await _produitRepository.GetAllProduitsAsync();
            if (produits == null || !produits.Any())
            {
                _logger.LogWarning("Aucun produit trouvé.");
                return NotFound("Aucun produit trouvé.");
            }

            // Génère un Id unique 
            produit.Id = produits.Max(p => p.Id) + 1;
            await _produitRepository.AddProduitAsync(produit);

            // Retourne l'objet créé avec l'url pour y accéder (best practice REST)
            return CreatedAtAction(nameof(GetProduitByIdAsync), new { id = produit.Id }, produit);
        }

        /// <summary>
        /// Met à jour un produit existant
        /// </summary>
        /// <param name="id">Identifiant du produit à modifier</param>
        /// <param name="produitMaj">Données du produit mises à jour</param>
        /// <returns>Code 204 si succès, 404 si non trouvé</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Produit>> UpdateProduitAsync(int id, [FromBody] Produit produitMaj)
        {
            if (produitMaj == null || id != produitMaj.Id)
                return BadRequest("Données incohérentes");

            _logger.LogInformation("Mise à jour du produit avec l'Id : {Id}", id);

            var produits = await _produitRepository.GetAllProduitsAsync();
            if (produits == null || !produits.Any())
            {
                _logger.LogWarning("Aucun produit trouvé.");
                return NotFound("Aucun produit trouvé.");
            }

            var produitExistant = produits.FirstOrDefault(p => p.Id == id);
            if (produitExistant == null)
                return NotFound($"Aucun produit avec l'Id {id}");

            produitExistant.Nom = produitMaj.Nom;
            produitExistant.Prix = produitMaj.Prix;
            produitExistant.Categorie = produitMaj.Categorie;

            return NoContent();
        }

        /// <summary>
        /// Supprime le produit existant
        /// </summary>
        /// <param name="id">Identifiant du produit à supprimer</param>
        /// <returns>Code 204 si succès, 404 si non trouvé</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduitAsync(int id)
        {
            _logger.LogInformation("Suppression du produit avec l'Id : {Id}", id);

            var produits = await _produitRepository.GetAllProduitsAsync();
            if (produits == null || !produits.Any())
            {
                _logger.LogWarning("Aucun produit trouvé.");
                return NotFound("Aucun produit trouvé.");
            }
            
            var produit = produits.FirstOrDefault(p => p.Id == id);
            if (produit == null)
                return NotFound($"Aucun produit avec l'Id {id}");

            await _produitRepository.DeleteProduitAsync(id);

            return NoContent();
        }

        /// <summary>
        /// Recherche les produits selon différents critères
        /// </summary>
        /// <param name="categorie">Catégorie du produit à filtrer</param>
        /// <param name="prixMax">Prix max du produit</param>
        /// <param name="searchItem">caractère de recherche par nom du produit</param>
        /// <returns>Liste de produits filtrés</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Produit>>> GetByCategorieAsync(
            [FromQuery] string? categorie,
            [FromQuery] decimal? prixMax,
            [FromQuery] string? searchItem)
        {
            _logger.LogInformation("Recherche de produits - catégorie: {Categorie}, prixMax: {PrixMax}, searchItem: {SearchItem}",
                                    categorie, prixMax, searchItem);

            if (string.IsNullOrWhiteSpace(categorie) && !prixMax.HasValue && string.IsNullOrWhiteSpace(searchItem))
            {
                _logger.LogWarning("Aucun critère de recherche fourni.");
                return BadRequest("Veuillez fournir au moins un critère de recherche.");
            }

            var produits = await _produitRepository.GetAllProduitsAsync();
            if (produits == null || !produits.Any())
            {
                _logger.LogWarning("Aucun produit trouvé.");
                return NotFound("Aucun produit trouvé.");
            }
            
            var result = produits.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(categorie))
                result = result.Where(p => p.Categorie.ToLower() == categorie.ToLower());

            if (prixMax.HasValue)
                result = result.Where(p => p.Prix <= prixMax.Value);

            if (!string.IsNullOrWhiteSpace(searchItem))
                result = result.Where(p => p.Nom.ToLower().Contains(searchItem.ToLower()));

            return Ok(result);
        }

        [HttpGet("group-by-categorie")]
        public async Task<IActionResult> GetProduitsGroupesParCategorieAsync()
        {
            _logger.LogInformation("Récupération des produits groupés par catégorie");

            var produits = await _produitRepository.GetAllProduitsAsync();
            if (produits == null || !produits.Any())
            {
                _logger.LogWarning("Aucun produit trouvé.");
                return NotFound("Aucun produit trouvé.");
            }
            
            var groupes = produits
                .GroupBy(p => p.Categorie)
                .Select(g => new
                {
                    Categorie = g.Key,
                    NombreProduits = g.Count(),
                    Produits = g.Select(p => new { p.Nom, p.Prix })
                });

            return Ok(groupes);
        }

        [HttpGet("produits-avec-categorie")]
        public async Task<IActionResult> GetProduitsAvecDetailsCategorieAsync()
        {
            _logger.LogInformation("Récupération des produits avec détails de catégorie");

            var produits = await _produitRepository.GetAllProduitsAsync();
            if (produits == null || !produits.Any())
            {
                _logger.LogWarning("Aucun produit trouvé.");
                return NotFound("Aucun produit trouvé.");
            }
            
            var resultats = produits
                .Join(Categories,
                    produit => produit.Categorie,
                    categorie => categorie.Nom,
                    (produit, categorie) => new
                    {
                        produit.Nom,
                        produit.Prix,
                        Categorie = categorie.Nom,
                        categorie.Description
                    });

            return Ok(resultats);
        }

        [HttpGet("stats-prix")]
        public async Task<IActionResult> GetStatistiquesPrixAsync()
        {
            _logger.LogInformation("Calcul des statistiques de prix des produits");

            var produits = await _produitRepository.GetAllProduitsAsync();
            if (produits == null || !produits.Any())
            {
                _logger.LogWarning("Aucun produit trouvé.");
                return NotFound("Aucun produit trouvé.");
            }
            
            var stats = produits
                .GroupBy(p => p.Categorie)
                .Select(g => new
                {
                    Categorie = g.Key,
                    NombreProduits = g.Count(),
                    PrixMoyen = g.Average(p => p.Prix),
                    PrixMax = g.Max(p => p.Prix),
                    PrixMin = g.Min(p => p.Prix)
                });

            return Ok(stats);
        }

        [HttpGet("filtre")]
        public async Task<IActionResult> FiltrerProduitsAsync(
            [FromQuery] decimal? minPrix,
            [FromQuery] decimal? maxPrix,
            [FromQuery] string? categorie)
        {
            _logger.LogInformation("Filtrage des produits - minPrix: {MinPrix}, maxPrix: {MaxPrix}, catégorie: {Categorie}",
                                    minPrix, maxPrix, categorie);
            
            if (!minPrix.HasValue && !maxPrix.HasValue && string.IsNullOrEmpty(categorie))
            {
                _logger.LogWarning("Aucun critère de filtrage fourni.");
                return BadRequest("Veuillez fournir au moins un critère de filtrage.");
            }

            var produits = await _produitRepository.GetAllProduitsAsync();
            if (produits == null || !produits.Any())
            {
                _logger.LogWarning("Aucun produit trouvé.");
                return NotFound("Aucun produit trouvé.");
            }

            var produitsFiltres = produits
                .Where(p =>
                    (!minPrix.HasValue || p.Prix >= minPrix) &&
                    (!maxPrix.HasValue || p.Prix <= maxPrix) &&
                    (string.IsNullOrEmpty(categorie) || p.Categorie == categorie)
                );

            return Ok(produitsFiltres);
        }

    }
}