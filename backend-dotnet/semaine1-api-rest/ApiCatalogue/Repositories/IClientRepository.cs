using ApiCatalogue.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiCatalogue.Repositories
{
    public interface IClientRepository
    {
        /// <summary>
        /// Gets all clients.
        /// </summary>
        /// <returns>A list of clients.</returns>
        Task<IEnumerable<Client>> GetAllClientsAsync();
    }
}