// using ApiCatalogue.Repositories;
// using ApiCatalogue.Repositories.InMemory;
// using ApiCatalogue.Data;
// using Microsoft.EntityFrameworkCore;

// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddControllers();
// builder.Services.AddEndpointsApiExplorer();
// //builder.Services.AddSwaggerGen();
// builder.Services.AddSwaggerGen();

// builder.Services.AddScoped<IClientRepository, ClientRepository>();
// builder.Services.AddScoped<IProduitRepository, ProduitRepository>();
// builder.Services.AddScoped<IAchatRepository, AchatRepository>();

// builder.Services.AddDbContext<CatalogueDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();

//     //a ajouter temporairement
//     app.UseDeveloperExceptionPage();
// }

// app.UseHttpsRedirection();

// //facultatif mais recommandé si tu utilises [Authorize] plus tard
// app.UseAuthorization();

// //Obligatoire pour que les routes soient actives
// app.MapControllers();


// app.Run();



using ApiCatalogue.Repositories;
using ApiCatalogue.Repositories.InMemory;
using ApiCatalogue.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCatalogue;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<IClientRepository, ClientRepository>();
        builder.Services.AddScoped<IProduitRepository, ProduitRepository>();
        builder.Services.AddScoped<IAchatRepository, AchatRepository>();

        if (builder.Environment.IsEnvironment("Testing"))
        {
            builder.Services.AddDbContext<CatalogueDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));
        }
        else
        {
            builder.Services.AddDbContext<CatalogueDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        }

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
