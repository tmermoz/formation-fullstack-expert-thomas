using ApiCatalogue.Data;
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
using FluentAssertions;

namespace TestsUnitaires.IntegrationTests
{
    public class AchatScenarioIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AchatScenarioIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ScenarioComplet_AchatClientProduit_Fonctionne()
        {
            // Étape 1 : Créer un produit
            var produit = new Produit { Nom = "TestProduit", Prix = 10.0m, Categorie = "Test" };
            var postProduit = await _client.PostAsJsonAsync("/api/Produits", produit);
            postProduit.EnsureSuccessStatusCode();
            var produitCree = await postProduit.Content.ReadFromJsonAsync<Produit>();

            // Étape 2 : Créer un client
            var client = new Client { Nom = "Jean", Age = 30, Ville = "Paris" };
            var postClient = await _client.PostAsJsonAsync("/api/Clients", client);
            postClient.EnsureSuccessStatusCode();
            var clientCree = await postClient.Content.ReadFromJsonAsync<Client>();

            // Étape 3 : Créer un achat
            var achat = new Achat
            {
                ClientId = clientCree.Id,
                ProduitId = produitCree.Id,
                Quantite = 3,
                DateAchat = DateTime.UtcNow
            };

            var postAchat = await _client.PostAsJsonAsync("/api/Achats", achat);
            postAchat.EnsureSuccessStatusCode();
            var achatCree = await postAchat.Content.ReadFromJsonAsync<Achat>();

            // Étape 4 : Vérifier l’achat enregistré
            var getAchat = await _client.GetAsync($"/api/Achats/{achatCree.Id}");
            getAchat.EnsureSuccessStatusCode();
            var achatRetrouve = await getAchat.Content.ReadFromJsonAsync<Achat>();

            achatRetrouve.Should().NotBeNull();
            achatRetrouve.ClientId.Should().Be(clientCree.Id);
            achatRetrouve.ProduitId.Should().Be(produitCree.Id);
            achatRetrouve.Quantite.Should().Be(3);
        }
    }
}