// InMemory/ClientRepository.cs
using System.Collections.Generic;
using ApiCatalogue.Models;
using ApiCatalogue.Repositories;
using System.Linq;
using System.Threading.Tasks;

// <summary>
// InMemory implementation of the IClientRepository interface.  
// This repository uses a static list to simulate a database.
// </summary>
namespace ApiCatalogue.Repositories.InMemory
{
    public class ClientRepository : IClientRepository
    {
        private static readonly List<Client> Clients = new()
        {
            new() { Nom = "Alice", Age = 30, Ville = "Paris" },
            new() { Nom = "Bob", Age = 42, Ville = "Lyon" },
            new() { Nom = "Claire", Age = 25, Ville = "Paris" },
            new() { Nom = "David", Age = 35, Ville = "Toulouse" },
        };

        public Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return Task.FromResult(GetAllClients());
        }

        public IEnumerable<Client> GetAllClients() => Clients;

    }
}

