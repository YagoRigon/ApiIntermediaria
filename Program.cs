using ApiIntermediaria.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Adiciona o HttpClient para fazer requisições externas
builder.Services.AddHttpClient<RepositorioController>();

// Adiciona os serviços de controladores (Web API)
builder.Services.AddControllers();

var app = builder.Build();

// Configuração de endpoint para controladores
app.MapControllers();

app.Run();
