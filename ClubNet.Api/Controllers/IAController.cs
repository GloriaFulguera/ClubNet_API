using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace ClubNet.Api.Controllers
{
    [ApiController]
    [Route("api/ia")]
    public class IAController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private const string OPENROUTER_URL = "https://openrouter.ai/api/v1/chat/completions";
        private const string OPENROUTER_MODELS_URL = "https://openrouter.ai/api/v1/models";

        public IAController(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        [HttpPost("sugerirActividad")]
        public async Task<IActionResult> SugerirActividad([FromBody] UsuarioDatos datos)
        {
            var apiKey = _config["OpenRouter:ApiKey"];

            // Obtener lista de modelos gratuitos
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                var modelsResponse = await _httpClient.GetAsync(OPENROUTER_MODELS_URL);
                var modelsJson = await modelsResponse.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(modelsJson);

                var modelosGratis = doc.RootElement.GetProperty("data")
                    .EnumerateArray()
                    .Where(m => m.GetProperty("id").GetString()?.EndsWith(":free") == true)
                    .Select(m => m.GetProperty("id").GetString())
                    .ToList();

                string modeloElegido = modelosGratis.Count > 0
                    ? modelosGratis[new Random().Next(modelosGratis.Count)]
                    : "gpt-3.5-turbo"; // fallback si no hay gratis

                // Preparar prompt
                var prompt = $@"
                    Eres un asistente de un club deportivo. Basado en los datos del usuario:
                    Edad: {datos.Edad}
                    Intereses: {string.Join(", ", datos.Intereses ?? new string[0])}
                    Historial: {string.Join(", ", datos.Historial ?? new string[0])}
                    Sugiere una sola actividad deportiva disponible en el club que le podría gustar y explica brevemente por qué.";

                var body = new
                {
                    model = modeloElegido,
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    },
                    max_tokens = 80,
                    temperature = 0.7
                };

                var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(OPENROUTER_URL, content);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return Ok(new { sugerencia = "Ocurrio un error ¡Perdon!" });

                using var resultDoc = JsonDocument.Parse(json);
                var suggestion = resultDoc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return Ok(new { sugerencia = suggestion, modeloUsado = modeloElegido });
            }
            catch
            {
                // Si algo falla
                return Ok(new { sugerencia = "Ocurrió un error al generar la sugerencia.", modeloUsado = "error" });
            }
        }
    }

    public class UsuarioDatos
    {
        public int Edad { get; set; }
        public string[] Intereses { get; set; }
        public string[] Historial { get; set; }
    }
}
