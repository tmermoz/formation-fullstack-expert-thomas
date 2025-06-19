namespace ApiCatalogue.Models
{
     /// <summary>
    /// Représente un produit dans le catalogue.
    /// </summary>
    public class Produit
    {
        /// <summary>
        /// Identifiant unique du produit.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nom du produit.
        /// </summary>
        public string Nom { get; set; } = string.Empty;

        /// <summary>
        /// Prix du produit.
        /// </summary>
        public decimal Prix { get; set; }

        /// <summary>
        /// Catégorie du produit.
        /// </summary>
        public string Categorie { get; set; } = string.Empty;
    }
}