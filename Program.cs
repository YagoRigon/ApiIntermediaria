using ApiIntermediaria.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Adiciona o HttpClient para fazer requisi��es externas
builder.Services.AddHttpClient<RepositorioController>();

// Adiciona os servi�os de controladores (Web API)
builder.Services.AddControllers();

var app = builder.Build();

// Configura��o de endpoint para controladores
app.MapControllers();

app.Run();
