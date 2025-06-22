using ApiCatalogue.Repositories;
using ApiCatalogue.Repositories.InMemory;
using ApiCatalogue.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IProduitRepository, ProduitRepository>();
builder.Services.AddScoped<IAchatRepository, AchatRepository>();

builder.Services.AddDbContext<CatalogueDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    //a ajouter temporairement
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

//facultatif mais recommandé si tu utilises [Authorize] plus tard
app.UseAuthorization();

//Obligatoire pour que les routes soient actives
app.MapControllers();


app.Run();
