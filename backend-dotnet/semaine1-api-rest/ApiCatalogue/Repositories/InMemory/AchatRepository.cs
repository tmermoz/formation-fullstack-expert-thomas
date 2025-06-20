using System.Collections.Generic;
using ApiCatalogue.Models;
using ApiCatalogue.Repositories;
using System.Linq;
using System.Threading.Tasks;

// <summary>
// InMemory implementation of the IAchatRepository interface.  
// This repository uses a static list to simulate a database.
// </summary>
namespace ApiCatalogue.Repositories.InMemory
{
    public class AchatRepository : IAchatRepository
    {
        private static readonly List<Achat> Achats = new()
        {
            new Achat { NomClient = "Alice", NomProduit = "Ordinateur portable" },
            new Achat { NomClient = "Bob", NomProduit = "Smartphone" },
            new Achat { NomClient = "Claire", NomProduit = "Ordinateur portable" },
            new Achat { NomClient = "David", NomProduit = "Ecouteurs bluetooth" },
            new Achat { NomClient = "Alice", NomProduit = "Ecouteurs bluetooth" },
        };

        public Task<IEnumerable<Achat>> GetAllAchatsAsync()
        {
            return Task.FromResult(GetAllAchats());
        }

        public IEnumerable<Achat> GetAllAchats() => Achats;

    }
}

