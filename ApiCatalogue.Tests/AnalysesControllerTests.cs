using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using ApiCatalogue.Controllers;
using ApiCatalogue.Models;
using ApiCatalogue.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ApiCatalogue.Tests
{
    public class AnalysesControllerTests
    {
        [Fact]
        public void TestAvecPause()
        {
            var x = 42; // pose le point d’arrêt ici
            System.Threading.Thread.Sleep(10000);
            Assert.Equal(42, x);
        }

        /// <summary>
        /// Tests the <see cref="AnalysesController.GetTopClientsBySpending(int)"/> method to ensure it returns an <see cref="OkObjectResult"/>
        /// containing the correct number of top clients based on spending.
        /// </summary>
        /// <remarks>
        /// This test verifies that:
        /// <list type="bullet">
        /// <item>The action returns an <see cref="OkObjectResult"/>.</item>
        /// <item>The result contains an <see cref="IEnumerable{TopClientDto}"/>.</item>
        /// <item>The number of clients returned matches the requested count.</item>
        /// <item>Each client has a non-empty <c>NomClient</c> property.</item>
        /// </list>
        /// </remarks>
        [Fact]
        public void GetTopClientsBySpending_ReturnsOkWithCorrectNumberOfClients()
        {
            System.Threading.Thread.Sleep(10000); // 10 secondes
            
            // Arrange
            var mockLogger = new Mock<ILogger<AnalysesController>>();
            var controller = new AnalysesController(mockLogger.Object);

            // Act
            var result = controller.GetTopClientsBySpending(2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result); // ✅ On accède à .Result  ici car ActionResult<T>
            var clients = Assert.IsAssignableFrom<IEnumerable<TopClientDto>>(okResult.Value);

            Assert.Equal(2, clients.Count());
            Assert.All(clients, c => Assert.False(string.IsNullOrWhiteSpace(c.NomClient)));
            // Optionnel : test plus poussé sur les noms ou montants
        }
    }
}
