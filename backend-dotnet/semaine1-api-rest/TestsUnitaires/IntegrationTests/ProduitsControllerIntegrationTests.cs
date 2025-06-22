using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ApiCatalogue;
using ApiCatalogue.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace TestsUnitaires.IntegrationTests
{
    public class ProduitsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ProduitsControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remplace le contexte SQL par une DB en mémoire
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<CatalogueDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<CatalogueDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb"));

                    // Seed de données
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<CatalogueDbContext>();

                    db.Database.EnsureCreated();

                    db.Produits.AddRange(new List<Produit>
                    {
                        new Produit { Id = 1, Nom = "Produit A", Categorie = "Tech", Prix = 50 },
                        new Produit { Id = 2, Nom = "Produit B", Categorie = "Maison", Prix = 20 },
                    });

                    db.SaveChanges();
                });
            });
        }

        [Fact]
        public async Task GetProduits_WithValidCategorie_ReturnsExpectedProducts()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/Produits?categorie=Tech");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var produits = await response.Content.ReadFromJsonAsync<List<Produit>>();
            produits.Should().HaveCount(1);
            produits[0].Nom.Should().Be("Produit A");
        }

        [Fact]
        public async Task GetProduits_WithMissingCategorie_ReturnsBadRequest()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/Produits");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetProduits_WithUnknownCategorie_ReturnsNotFound()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/Produits?categorie=Inconnue");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task PostProduit_ShouldReturnCreatedProduit()
        {
            // Arrange
            var newProduit = new Produit
            {
                Nom = "TestProduit",
                Categorie = "TestCatégorie",
                Prix = 9.99M
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Produits", newProduit);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            
            var produitCree = await response.Content.ReadFromJsonAsync<Produit>();
            produitCree.Should().NotBeNull();
            produitCree!.Nom.Should().Be("TestProduit");
            produitCree.Categorie.Should().Be("TestCatégorie");
            produitCree.Prix.Should().Be(9.99M);
        }
    }
}
