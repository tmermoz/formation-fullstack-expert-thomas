// InMemory/ClientRepository.cs
using System.Collections.Generic;
using ApiCatalogue.Models;
using ApiCatalogue.Repositories;
using System.Linq;
using System.Threading.Tasks;
using ApiCatalogue.Data;
using Microsoft.EntityFrameworkCore;

// <summary>
// InMemory implementation of the IClientRepository interface.  
// This repository uses a static list to simulate a database.
// </summary>
namespace ApiCatalogue.Repositories.InMemory
{
    public class ClientRepository : IClientRepository
    {
        private readonly CatalogueDbContext _context;

        public ClientRepository(CatalogueDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return await _context.Clients.ToListAsync();
        }
        
    }
}

