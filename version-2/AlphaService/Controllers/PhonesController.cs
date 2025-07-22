using AlphaService.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;

namespace AlphaService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhonesController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _betaBaseUrl;

        public PhonesController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _betaBaseUrl = configuration["BetaService:BaseUrl"];
        }

        [HttpGet("/health")]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                service = "AlphaService",
                status = "UP",
                timestamp = DateTime.UtcNow
            });
        }


        // ✅ Helper method to log request and response
        private void LogRequestAndResponse(string method, string url, object? requestBody, HttpResponseMessage response, string responseData)
        {
            object? parsedResponse;
            try
            {
                parsedResponse = JsonSerializer.Deserialize<object>(responseData);
            }
            catch
            {
                parsedResponse = responseData;
            }

            Log.ForContext("Method", method)
               .ForContext("Url", url)
               .ForContext("RequestBody", requestBody != null ? JsonSerializer.Serialize(requestBody) : "None")
               .ForContext("StatusCode", (int)response.StatusCode)
               .ForContext("ResponseBody", parsedResponse)
               .Information("HTTP {Method} | URL: {Url} | Status: {StatusCode}");
        }

        private void LogError(string method, string url, object? requestBody, Exception ex)
        {
            Log.ForContext("Method", method)
               .ForContext("Url", url)
               .ForContext("RequestBody", requestBody != null ? JsonSerializer.Serialize(requestBody) : "None")
               .Error(ex, "❌ ERROR during {Method} {Url}");
        }


        // GET: api/phones
        [HttpGet]
        public async Task<IActionResult> GetPhones()
        {
            string url = _betaBaseUrl;
            try
            {
                var response = await _httpClient.GetAsync(url);
                var data = await response.Content.ReadAsStringAsync();
                LogRequestAndResponse("GET", url, null, response, data);
                return StatusCode((int)response.StatusCode, data);
            }
            catch (Exception ex)
            {
                LogError("GET", url, null, ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // GET: api/phones/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPhone(int id)
        {
            string url = $"{_betaBaseUrl}/{id}";
            try
            {
                var response = await _httpClient.GetAsync(url);
                var data = await response.Content.ReadAsStringAsync();
                LogRequestAndResponse("GET", url, null, response, data);
                return StatusCode((int)response.StatusCode, data);
            }
            catch (Exception ex)
            {
                LogError("GET", url, null, ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // POST: api/phones
        [HttpPost]
        public async Task<IActionResult> AddPhone([FromBody] Phone phone)
        {
            string url = _betaBaseUrl;
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, phone);
                var data = await response.Content.ReadAsStringAsync();
                LogRequestAndResponse("POST", url, phone, response, data);
                return StatusCode((int)response.StatusCode, data);
            }
            catch (Exception ex)
            {
                LogError("POST", url, phone, ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // PUT: api/phones/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePhone(int id, [FromBody] Phone phone)
        {
            string url = $"{_betaBaseUrl}/{id}";
            try
            {
                var response = await _httpClient.PutAsJsonAsync(url, phone);
                var data = await response.Content.ReadAsStringAsync();
                LogRequestAndResponse("PUT", url, phone, response, data);
                return StatusCode((int)response.StatusCode, data);
            }
            catch (Exception ex)
            {
                LogError("PUT", url, phone, ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // DELETE: api/phones/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhone(int id)
        {
            string url = $"{_betaBaseUrl}/{id}";
            try
            {
                var response = await _httpClient.DeleteAsync(url);
                var data = await response.Content.ReadAsStringAsync();
                LogRequestAndResponse("DELETE", url, null, response, data);
                return StatusCode((int)response.StatusCode, data);
            }
            catch (Exception ex)
            {
                LogError("DELETE", url, null, ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
