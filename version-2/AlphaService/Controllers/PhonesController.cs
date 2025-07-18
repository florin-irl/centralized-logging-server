using AlphaService.Models;
using Microsoft.AspNetCore.Mvc;

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

        // GET: api/phones
        [HttpGet]
        public async Task<IActionResult> GetPhones()
        {
            var response = await _httpClient.GetAsync(_betaBaseUrl);
            var data = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, data);
        }

        // GET: api/phones/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPhone(int id)
        {
            var response = await _httpClient.GetAsync($"{_betaBaseUrl}/{id}");
            var data = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, data);
        }

        // POST: api/phones
        [HttpPost]
        public async Task<IActionResult> AddPhone([FromBody] Phone phone)
        {
            var response = await _httpClient.PostAsJsonAsync(_betaBaseUrl, phone);
            var data = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, data);
        }

        // PUT: api/phones/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePhone(int id, [FromBody] Phone phone)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_betaBaseUrl}/{id}", phone);
            var data = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, data);
        }

        // DELETE: api/phones/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhone(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_betaBaseUrl}/{id}");
            var data = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, data);
        }
    }
}
