using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using ApiCatalogue.Controllers;
using ApiCatalogue.Repositories;
using ApiCatalogue.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestsUnitaires.Controllers
{
    public class ClientsControllerTests
    {
        private readonly ClientsController _controller;
        private readonly Mock<IClientRepository> _mockRepo;
        private readonly Mock<ILogger<ClientsController>> _mockLogger;

        public ClientsControllerTests()
        {
            _mockRepo = new Mock<IClientRepository>();
            _mockLogger = new Mock<ILogger<ClientsController>>();
            _controller = new ClientsController(_mockLogger.Object, _mockRepo.Object);
        }

        [Fact]
        public async Task GetClients_ReturnsListOfClients()
        {
            // Arrange
            var fakeClients = new List<Client>
            {
                new Client { Nom = "Alice", Age = 30, Ville = "Paris" },
                new Client { Nom = "Bob", Age = 42, Ville = "Lyon" },
            };

            _mockRepo.Setup(repo => repo.GetAllClientsAsync()).ReturnsAsync(fakeClients);

            // Act
            var result = await _controller.GetClients("Paris", 35); // paramètres valides

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var noms = Assert.IsAssignableFrom<IEnumerable<string>>(okResult.Value);
            Assert.Contains("Alice", noms);
        }

        [Fact]
        public async Task GetClients_ShouldReturnFilteredClients_WhenVilleAndAgeAreValid()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetAllClientsAsync()).ReturnsAsync(new List<Client>
            {
                new Client { Nom = "Alice", Age = 30, Ville = "Paris" },
                new Client { Nom = "Claire", Age = 25, Ville = "Paris" },
                new Client { Nom = "Bob", Age = 42, Ville = "Lyon" },
            });

            var ville = "Paris";
            var age = 35;

            // Act
            var actionResult = await _controller.GetClients(ville, age);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var noms = Assert.IsAssignableFrom<IEnumerable<string>>(okResult.Value);
            Assert.Contains("Alice", noms);
            Assert.Contains("Claire", noms);
            Assert.Equal(2, noms.Count());
        }
    }
}
