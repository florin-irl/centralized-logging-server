using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_service.DTOs;
using dotnet_service.Models;
using dotnet_service.Repositories;
using Microsoft.Extensions.Logging;

namespace dotnet_service.Services
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _repository;
        private readonly ILogger<LogService> _logger;

        public LogService(ILogRepository repository, ILogger<LogService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<LogDto>> GetAllAsync()
        {
            var logs = await _repository.GetAllAsync();
            _logger.LogInformation("Fetched all logs. Count: {Count}", logs.Count());
            return logs.Select(log => new LogDto
            {
                Id = log.Id,
                Message = log.Message,
                Level = log.Level,
                Timestamp = log.Timestamp
            });
        }

        public async Task<LogDto> GetByIdAsync(int id)
        {
            var log = await _repository.GetByIdAsync(id);
            if (log == null)
            {
                _logger.LogWarning("Log entry with ID {Id} not found.", id);
                return null;
            }

            _logger.LogInformation("Fetched log entry with ID {Id}.", id);
            return new LogDto
            {
                Id = log.Id,
                Message = log.Message,
                Level = log.Level,
                Timestamp = log.Timestamp
            };
        }

        public async Task CreateAsync(CreateLogDto dto)
        {
            var log = new LogEntry
            {
                Message = dto.Message,
                Level = dto.Level,
                Timestamp = DateTime.UtcNow
            };

            await _repository.AddAsync(log);
            _logger.LogInformation("Created new log entry: {@Log}", log);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
            _logger.LogInformation("Deleted log entry with ID {Id}.", id);
        }
    }
}
