using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using ApiCatalogue.Controllers;
using ApiCatalogue.Repositories;
using ApiCatalogue.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestsUnitaires.Controllers
{
    public class ProduitsControllerTests
    {
        private readonly ProduitsController _controller;
        private readonly Mock<IProduitRepository> _produitRepositoryMock;

        public ProduitsControllerTests()
        {
            _produitRepositoryMock = new Mock<IProduitRepository>();
            var loggerMock = new Mock<ILogger<ProduitsController>>();
            _controller = new ProduitsController(loggerMock.Object, _produitRepositoryMock.Object);
        }

        [Fact]
        public async Task GetProduits_WithValidCategorie_ReturnsFilteredProduits()
        {
            // Arrange
            var produits = new List<Produit>
            {
                new Produit { Id = 1, Nom = "PC", Categorie = "Informatique", Prix = 1000 },
                new Produit { Id = 2, Nom = "Imprimante", Categorie = "Informatique", Prix = 300 },
                new Produit { Id = 3, Nom = "Chaise", Categorie = "Mobilier", Prix = 80 }
            };

            _produitRepositoryMock.Setup(repo => repo.GetAllProduitsAsync()).ReturnsAsync(produits);

            // Act
            var result = await _controller.GetProduits("Informatique");

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var returnedProduits = okResult.Value as IEnumerable<Produit>;
            returnedProduits.Should().HaveCount(2);
            returnedProduits.Should().OnlyContain(p => p.Categorie == "Informatique");
        }

        [Fact]
        public async Task GetProduits_WithNullCategorie_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetProduits(null);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}