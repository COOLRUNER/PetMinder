using Microsoft.EntityFrameworkCore;
using PetMinder.Data;

namespace PetMinder.Api.Services
{
    public class DatabaseMaintenanceService : IDatabaseMaintenanceService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseMaintenanceService> _logger;

        public DatabaseMaintenanceService(ApplicationDbContext context, ILogger<DatabaseMaintenanceService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task PokeDatabaseAsync()
        {
            try
            {
                await _context.Users.AsNoTracking().AnyAsync();
                _logger.LogInformation("Database keep-alive: Poked successfully at {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database keep-alive: Failed to poke database.");
            }
        }
    }
}
