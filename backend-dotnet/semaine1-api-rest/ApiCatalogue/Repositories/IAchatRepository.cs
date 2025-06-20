namespace ApiCatalogue.Repositories
{
    using ApiCatalogue.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for the Achat repository.
    /// </summary>
    public interface IAchatRepository
    {
        /// <summary>
        /// Gets all achats.
        /// </summary>
        /// <returns>A list of achats.</returns>
        Task<IEnumerable<Achat>> GetAllAchatsAsync();
        
    }
}