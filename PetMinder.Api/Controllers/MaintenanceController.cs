using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetMinder.Data;
using System.Text;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public MaintenanceController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("heartbeat")]
        public async Task<IActionResult> Heartbeat()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return Unauthorized("Basic Auth required");
            }

            var authHeaderValue = authHeader.ToString();
            if (!authHeaderValue.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized("Basic Auth required");
            }

            try
            {
                var base64Credentials = authHeaderValue.Substring(6).Trim();
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(base64Credentials)).Split(':');
                if (credentials.Length < 2)
                {
                    return Unauthorized("Invalid credentials format");
                }
                
                var username = credentials[0];
                var password = credentials[1];

                var expectedUser = _configuration["Maintenance:CronUser"] ?? "pm-cron";
                var expectedPass = _configuration["Maintenance:CronPass"] ?? "pm-secret-ping-2026";

                if (username != expectedUser || password != expectedPass)
                {
                    return Unauthorized("Invalid credentials");
                }
                await _context.Users.AnyAsync();

                return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
            }
            catch
            {
                return Unauthorized("Invalid auth header format");
            }
        }
    }
}
