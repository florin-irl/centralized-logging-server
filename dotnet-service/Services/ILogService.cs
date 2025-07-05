using System.Collections.Generic;
using System.Threading.Tasks;
using dotnet_service.DTOs;

namespace dotnet_service.Services
{
    public interface ILogService
    {
        Task<IEnumerable<LogDto>> GetAllAsync();
        Task<LogDto> GetByIdAsync(int id);
        Task CreateAsync(CreateLogDto dto);
        Task DeleteAsync(int id);
    }
}
