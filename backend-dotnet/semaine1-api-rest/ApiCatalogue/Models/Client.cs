namespace ApiCatalogue.Models
{
    /// <summary>
    /// Représente les clients
    /// </summary>
    public class Client
    {
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