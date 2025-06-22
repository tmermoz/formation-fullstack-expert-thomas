using System.ComponentModel.DataAnnotations;

namespace ApiCatalogue.Models
{
    public class Achat
    {
        [Key]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ProduitId { get; set; }
        public int Quantite { get; set; }
        public DateTime DateAchat { get; set; }

        // (facultatif : navigation properties si utiles)
        public Client? Client { get; set; }
        public Produit? Produit { get; set; }
    }
}