using dotnet_service.DTOs;
using dotnet_service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace dotnet_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly ILogger<LogsController> _logger;

        public LogsController(ILogService logService, ILogger<LogsController> logger)
        {
            _logService = logService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GET /api/logs called.");
            var logs = await _logService.GetAllAsync();
            return Ok(logs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("GET /api/logs/{Id} called.", id);
            var log = await _logService.GetByIdAsync(id);
            if (log == null)
            {
                _logger.LogWarning("GET /api/logs/{Id} returned 404.", id);
                return NotFound();
            }
            return Ok(log);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLogDto dto)
        {
            _logger.LogInformation("POST /api/logs called with body: {@Dto}", dto);
            await _logService.CreateAsync(dto);
            return Created("", null);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("DELETE /api/logs/{Id} called.", id);
            await _logService.DeleteAsync(id);
            return NoContent();
        }
    }
}
