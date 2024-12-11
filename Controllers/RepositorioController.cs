using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace ApiIntermediaria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepositorioController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public RepositorioController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Endpoint para retornar os repositórios
        [HttpGet("repositorios")]
        public async Task<IActionResult> GetRepositorios()
        {
            try
            {
                // URL da API externa
                var apiUrl = "https://githubrepoapi-eecgh9g4fpesebet.brazilsouth-01.azurewebsites.net/api/Repositories/chsarp";

                // Fazendo a requisição para a API externa
                var response = await _httpClient.GetStringAsync(apiUrl);

                // Verifica se a resposta é nula ou vazia
                if (string.IsNullOrEmpty(response))
                {
                    return StatusCode(500, new { message = "A resposta da API externa está vazia." });
                }

                // Tentando deserializar a resposta
                ApiResponse repositoriosResponse = null;
                try
                {
                    repositoriosResponse = JsonConvert.DeserializeObject<ApiResponse>(response);
                }
                catch (JsonException jsonEx)
                {
                    return StatusCode(500, new { message = "Erro ao deserializar a resposta da API externa", error = jsonEx.Message });
                }

                // Verifica se a lista de repositórios é nula ou vazia
                if (repositoriosResponse?.Repositorios == null || repositoriosResponse.Repositorios.Count == 0)
                {
                    return NotFound(new { message = "Nenhum repositório encontrado." });
                }

                // Formata os dados dos repositórios
                var repositoriosFormatados = repositoriosResponse.Repositorios
                    .Select(repo => new
                    {
                        title = repo.Title,
                        imageUrl = repo.ImageUrl,
                        text = repo.Text
                    }).ToList();

                // Retorna os dados formatados
                return Ok(new { repositorios = repositoriosFormatados });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao acessar a API externa", error = ex.Message });
            }
        }

        // Modelo para mapear a resposta da API externa
        public class Repositorio
        {
            public string Title { get; set; }
            public string ImageUrl { get; set; }
            public string Text { get; set; }
        }

        public class ApiResponse
        {
            [JsonProperty("items")]
            public List<Repositorio> Repositorios { get; set; }
        }
    }
}
