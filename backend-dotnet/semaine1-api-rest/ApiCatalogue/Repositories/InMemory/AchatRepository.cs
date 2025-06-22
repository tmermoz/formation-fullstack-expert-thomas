using ApiCatalogue.Models;
using Microsoft.EntityFrameworkCore;
using ApiCatalogue.Data;

// <summary>
// InMemory implementation of the IAchatRepository interface.  
// This repository uses a static list to simulate a database.
// </summary>
namespace ApiCatalogue.Repositories.InMemory
{
    public class AchatRepository : IAchatRepository
    {
        private readonly CatalogueDbContext _context;

        public AchatRepository(CatalogueDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Achat>> GetAllAchatsAsync()
        {
            return await _context.Achats.ToListAsync();
        }

    }
}

