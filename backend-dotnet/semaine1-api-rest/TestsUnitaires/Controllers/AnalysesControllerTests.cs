using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using ApiCatalogue.Controllers;
using ApiCatalogue.Repositories;
using ApiCatalogue.Models;
using ApiCatalogue.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace TestsUnitaires.Controllers
{
    public class AnalysesControllerTests
    {
        private readonly Mock<ILogger<AnalysesController>> _mockLogger;
        private readonly Mock<IAchatRepository> _mockAchatRepo;
        private readonly Mock<IProduitRepository> _mockProduitRepo;
        private readonly Mock<IClientRepository> _mockClientRepo;
        private readonly AnalysesController _controller;

        public AnalysesControllerTests()
        {
            _mockLogger = new Mock<ILogger<AnalysesController>>();
            _mockAchatRepo = new Mock<IAchatRepository>();
            _mockProduitRepo = new Mock<IProduitRepository>();
            _mockClientRepo = new Mock<IClientRepository>();

            _controller = new AnalysesController(
                _mockLogger.Object,
                _mockClientRepo.Object,
                _mockProduitRepo.Object,
                _mockAchatRepo.Object
            );
        }

        [Fact]
        public async Task GetTopClientsBySpending_ReturnsTopClients_WhenDataIsValid()
        {
            // Arrange
            var achats = new List<Achat>
            {
                new Achat { NomClient = "Alice", NomProduit = "Ordinateur" },
                new Achat { NomClient = "Bob", NomProduit = "Ordinateur" },
                new Achat { NomClient = "Alice", NomProduit = "Souris" },
            };

            var produits = new List<Produit>
            {
                new Produit { Nom = "Ordinateur", Prix = 1000 },
                new Produit { Nom = "Souris", Prix = 50 }
            };

            _mockAchatRepo.Setup(r => r.GetAllAchatsAsync()).ReturnsAsync(achats);
            _mockProduitRepo.Setup(r => r.GetAllProduitsAsync()).ReturnsAsync(produits);

            // Act
            var result = await _controller.GetTopClientsBySpending(2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IEnumerable<TopClientDto>>(okResult.Value);
            var list = data.ToList();

            Assert.Equal(2, list.Count);
            Assert.Equal("Alice", list[0].NomClient);
            Assert.Equal(1050, list[0].TotalDepense);
            Assert.Equal("Bob", list[1].NomClient);
            Assert.Equal(1000, list[1].TotalDepense);
        }

        [Fact]
        public async Task GetTopClientsBySpending_ReturnsBadRequest_WhenNIsZero()
        {
            // Act
            var result = await _controller.GetTopClientsBySpending(0);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}
