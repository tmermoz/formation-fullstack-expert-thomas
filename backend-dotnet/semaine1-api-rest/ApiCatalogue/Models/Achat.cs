using System.ComponentModel.DataAnnotations;

namespace ApiCatalogue.Models
{
    public class Achat
    {
        [Key]
        public int Id { get; set; }  // ← Clé primaire requise
        public string NomClient { get; set; } = string.Empty;
        public string NomProduit { get; set; } = string.Empty;
    }
}