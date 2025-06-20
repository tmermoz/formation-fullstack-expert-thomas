using Microsoft.AspNetCore.Mvc;
using ApiCatalogue.Models;
using System.Collections.Generic;
using System.Linq;

namespace ApiCatalogue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProduitsController : ControllerBase
    {
        private readonly ILogger<ProduitsController> _logger;

        private static readonly List<Produit> Produits = new()
        {
            new Produit {Id = 1, Nom = "Ordinateur portable", Prix = 999.99m, Categorie = "Informatique"},
            new Produit {Id = 2, Nom = "Smartphone", Prix = 749.99m, Categorie = "Téléphonie"},
            new Produit {Id = 3, Nom = "Ecouteurs bluetooth", Prix = 129.00m, Categorie = "Audio"}
        };

        private static readonly List<Categorie> Categories = new()
        {
            new Categorie { Nom = "Informatique", Description = "Ordinateurs et périphériques" },
            new Categorie { Nom = "Téléphonie", Description = "Smartphones et accessoires" },
            new Categorie { Nom = "Audio", Description = "Équipements audio" }
        };

        public ProduitsController(ILogger<ProduitsController> logger)
        {
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
        public ActionResult<IEnumerable<Produit>> GetProduits([FromQuery] string? categorie)
        {
            //Console.WriteLine($"categorie : {categorie}");

            var produitsFiltres = string.IsNullOrEmpty(categorie)
                ? Produits
                : Produits
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
        public ActionResult<Produit> GetProduitById(int id)
        {
            var produit = Produits.FirstOrDefault(p => p.Id == id);
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
        public ActionResult<Produit> AddProduit([FromBody] Produit produit)
        {
            if (produit == null || string.IsNullOrWhiteSpace(produit.Nom))
                return BadRequest("Produit invalide");

            // Génère un Id unique 
            produit.Id = Produits.Max(p => p.Id) + 1;
            Produits.Add(produit);

            // Retourne l'objet créé avec l'url pour y accéder (best practice REST)
            return CreatedAtAction(nameof(GetProduitById), new { id = produit.Id }, produit);
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
        public ActionResult<Produit> UpdateProduit(int id, [FromBody] Produit produitMaj)
        {
            if (produitMaj == null || id != produitMaj.Id)
                return BadRequest("Données incohérentes");

            var produitExistant = Produits.FirstOrDefault(p => p.Id == id);
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
        public IActionResult DeleteProduit(int id)
        {
            var produit = Produits.FirstOrDefault(p => p.Id == id);
            if (produit == null)
                return NotFound($"Aucun produit avec l'Id {id}");

            Produits.Remove(produit);

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
        public ActionResult<IEnumerable<Produit>> GetByCategorie(
            [FromQuery] string? categorie,
            [FromQuery] decimal? prixMax,
            [FromQuery] string? searchItem)
        {
            var result = Produits.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(categorie))
                result = result.Where(p => p.Categorie.ToLower() == categorie.ToLower());

            if (prixMax.HasValue)
                result = result.Where(p => p.Prix <= prixMax.Value);

            if (!string.IsNullOrWhiteSpace(searchItem))
                result = result.Where(p => p.Nom.ToLower().Contains(searchItem.ToLower()));

            return Ok(result);
        }

        [HttpGet("group-by-categorie")]
        public IActionResult GetProduitsGroupesParCategorie()
        {
            var groupes = Produits
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
        public IActionResult GetProduitsAvecDetailsCategorie()
        {
            var resultats = Produits
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
        public IActionResult GetStatistiquesPrix()
        {
            var stats = Produits
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
        public IActionResult FiltrerProduits(
            [FromQuery] decimal? minPrix,
            [FromQuery] decimal? maxPrix,
            [FromQuery] string? categorie)
        {
            _logger.LogInformation("Filtrage des produits - minPrix: {MinPrix}, maxPrix: {MaxPrix}, catégorie: {Categorie}",
                                    minPrix, maxPrix, categorie);

            var produitsFiltres = Produits
                .Where(p =>
                    (!minPrix.HasValue || p.Prix >= minPrix) &&
                    (!maxPrix.HasValue || p.Prix <= maxPrix) &&
                    (string.IsNullOrEmpty(categorie) || p.Categorie == categorie)
                );

            return Ok(produitsFiltres);
        }

    }
}