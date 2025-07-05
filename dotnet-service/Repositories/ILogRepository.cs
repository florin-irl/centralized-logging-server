using System.Collections.Generic;
using System.Threading.Tasks;
using dotnet_service.Models;

namespace dotnet_service.Repositories
{
    public interface ILogRepository
    {
        Task<IEnumerable<LogEntry>> GetAllAsync();
        Task<LogEntry> GetByIdAsync(int id);
        Task AddAsync(LogEntry log);
        Task DeleteAsync(int id);
    }
}
