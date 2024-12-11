using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly string _externalApiUrl;

        // Construtor com injeção de dependência do HttpClient e IConfiguration
        public RepositorioController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _externalApiUrl = configuration["ApiSettings:ExternalApiUrl"]; // Obtém a URL da API externa
        }

        /// <summary>
        /// Endpoint para buscar e retornar os repositórios da API externa.
        /// </summary>
        [HttpGet("repositorios")]
        public async Task<IActionResult> GetRepositorios()
        {
            try
            {
                // Realiza a requisição à API externa e obtém a resposta em formato JSON
                var response = await _httpClient.GetStringAsync(_externalApiUrl);

                // Valida se a resposta está vazia ou nula
                if (string.IsNullOrWhiteSpace(response))
                {
                    return StatusCode(500, new { message = "A resposta da API externa está vazia." });
                }

                // Tenta deserializar o JSON da resposta para o modelo ApiResponse
                if (!TryDeserializeResponse(response, out ApiResponse repositoriosResponse))
                {
                    return StatusCode(500, new { message = "Erro ao deserializar a resposta da API externa." });
                }

                // Valida se a lista de repositórios retornada está vazia ou nula
                if (repositoriosResponse?.Repositorios == null || !repositoriosResponse.Repositorios.Any())
                {
                    return NotFound(new { message = "Nenhum repositório encontrado." });
                }

                // Formata os dados dos repositórios para retorno
                var repositoriosFormatados = repositoriosResponse.Repositorios
                    .Select(repo => new
                    {
                        repo.Title,
                        repo.ImageUrl,
                        repo.Text
                    }).ToList();

                return Ok(new { repositorios = repositoriosFormatados });
            }
            catch (HttpRequestException httpEx)
            {
                // Erro específico relacionado à requisição HTTP
                return StatusCode(500, new { message = "Erro ao acessar a API externa", error = httpEx.Message });
            }
            catch (Exception ex)
            {
                // Captura outros tipos de exceção
                return StatusCode(500, new { message = "Erro interno ao processar a requisição", error = ex.Message });
            }
        }

        /// <summary>
        /// Tenta deserializar o JSON da resposta para o modelo ApiResponse.
        /// </summary>
        private bool TryDeserializeResponse(string jsonResponse, out ApiResponse apiResponse)
        {
            try
            {
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
                return true;
            }
            catch (JsonException)
            {
                apiResponse = null;
                return false;
            }
        }

        // Modelo para mapear a resposta de um repositório
        public class Repositorio
        {
            public string Title { get; set; }
            public string ImageUrl { get; set; }
            public string Text { get; set; }
        }

        // Modelo para mapear a resposta completa da API externa
        public class ApiResponse
        {
            [JsonProperty("items")]
            public List<Repositorio> Repositorios { get; set; }
        }
    }
}
