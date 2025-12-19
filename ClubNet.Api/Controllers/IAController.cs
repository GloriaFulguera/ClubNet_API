using Microsoft.AspNetCore.Http; // Necesario para StatusCodes
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ClubNet.Models.DTO;
using System.Collections.Generic;

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

        /// <summary>
        /// Sugiere una actividad deportiva utilizando un modelo de Inteligencia Artificial (IA).
        /// </summary>
        /// <param name="datos">Datos del usuario (Edad, Intereses, Historial) para la sugerencia.</param>
        /// <returns>Un objeto que contiene la sugerencia de actividad y el modelo de IA utilizado.</returns>
        [HttpPost("sugerirActividad")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> SugerirActividad([FromBody] UsuarioDatos datos)
        {
            // ... implementation as before ...
            //
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

        /// <summary>
        /// Analiza las clases y el esfuerzo para dar recomendaciones al entrenador.
        /// </summary>
        [HttpPost("recomendar-planificacion")]
        // [Authorize(Roles = "Entrenador")] // <- ¡Descomenta esto cuando tengas la seguridad configurada!
        public async Task<IActionResult> RecomendarPlanificacion([FromBody] List<ClaseDTO> clases)
        {
            var apiKey = _config["OpenRouter:ApiKey"];

            if (clases == null || !clases.Any())
            {
                return BadRequest(new { mensaje = "No hay clases para analizar." });
            }

            try
            {
                // 1. Construir el resumen de las clases para el prompt
                var resumenClases = new StringBuilder();
                foreach (var c in clases)
                {
                    resumenClases.AppendLine($"- Actividad: {c.Actividad} | Clase: {c.Titulo} | Intensidad: {c.Intensidad} | Detalle: {c.Detalle}");
                }

                // 2. Crear el Prompt de experto
                var prompt = $@"
                Actúa como un Entrenador Deportivo Experto.
                Analiza la siguiente lista de clases:
                {resumenClases}

                Responde ÚNICAMENTE en formato HTML simple (sin markdown ```html, solo las etiquetas body).
                Usa esta estructura:
                <p>Breve resumen general.</p>
                <ul>
                    <li><strong>Análisis de Carga:</strong> Tu opinión.</li>
                    <li><strong>Riesgos:</strong> Posibles lesiones o fatiga.</li>
                    <li><strong>Recomendación:</strong> Tu consejo clave.</li>
                </ul>
                <p>Frase motivadora final.</p>";

                // 3. Configurar la petición a OpenRouter (Igual que en tu método anterior)
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var body = new
                {
                    model = "gpt-3.5-turbo", // O usa la lógica de modelos gratuitos que ya tienes
                    messages = new[]
                    {
                new { role = "system", content = "Eres un asistente experto en planificación deportiva." },
                new { role = "user", content = prompt }
            },
                    max_tokens = 300,
                    temperature = 0.7
                };

                var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(OPENROUTER_URL, content);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Error al consultar la IA");

                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var resultDoc = JsonDocument.Parse(jsonResponse);

                // Extraer el texto de la respuesta
                var recomendacion = resultDoc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return Ok(new { recomendacion });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", detalle = ex.Message });
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