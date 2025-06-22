using System.ComponentModel.DataAnnotations;

namespace ApiCatalogue.Models
{
    /// <summary>
    /// Représente les clients
    /// </summary>
    public class Client
    {
        [Key]
        public int Id { get; set; }  // ← Clé primaire requise
        
        /// <summary>
        /// Nom du client
        /// </summary>
        public string? Nom { get; set; }

        /// <summary>
        /// Age du client
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Ville du client
        /// </summary>
        public string? Ville { get; set; }
    }
}