using dotnet_service.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_service.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<LogEntry> Logs { get; set; }
    }
}
