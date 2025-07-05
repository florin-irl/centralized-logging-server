using System.Collections.Generic;
using System.Threading.Tasks;
using dotnet_service.Models;
using dotnet_service.Data;
using Microsoft.EntityFrameworkCore;

namespace dotnet_service.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly ApplicationDbContext _context;

        public LogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LogEntry>> GetAllAsync() => await _context.Logs.ToListAsync();

        public async Task<LogEntry> GetByIdAsync(int id) => await _context.Logs.FindAsync(id);

        public async Task AddAsync(LogEntry log)
        {
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var log = await _context.Logs.FindAsync(id);
            if (log != null)
            {
                _context.Logs.Remove(log);
                await _context.SaveChangesAsync();
            }
        }
    }
}
