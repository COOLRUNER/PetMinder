using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PetMinder.Data;
using PetMinder.Models;
using PetMinder.Shared.DTO;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly ApplicationDbContext _context;
        private readonly IVerificationService _verificationService;
        private readonly IReportService _reportService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IEmailService emailService, ApplicationDbContext context, 
            IVerificationService verificationService, IReportService reportService, IConfiguration configuration)
        {
            _authService = authService;
            _emailService = emailService;
            _context = context;
            _verificationService = verificationService;
            _reportService = reportService;
            _configuration = configuration;
        }

        [EnableRateLimiting("registration")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                await _authService.RegisterAsync(registerDTO);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while processing your request.",
                    details = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var token = await _authService.LoginAsync(loginDTO);
                return Ok(token);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var token = await _authService.RefreshTokenAsync(userId);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while refreshing token.", details = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] Guid token)
        {
            try
            {
                var success = await _emailService.VerifyEmailToken(token);

                if (success)
                {
                    return Ok(new { message = "Email verified successfully! 50 points have been awarded." });
                }
                else
                {
                    return BadRequest(new { message = "Invalid or expired verification token." });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "An error occurred during verification." });
            }
        }
        
        [Authorize]
        [HttpPost("send-verification-email")]
        public async Task<IActionResult> SendVerificationEmail()
        {
            try
            {
                var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var user = await _context.Users.FindAsync(userId);
        
                if (user == null) return NotFound();
        
                await _emailService.SendVerificationEmail(userId, user.Email);
        
                return Ok(new { message = "Verification email sent!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to resend verification email." });
            }
        }

        [Authorize]
        [HttpGet("verification-status")]
        public async Task<ActionResult<List<VerificationStep>>> GetVerificationStatus()
        {
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var status = await _verificationService.GetCompletedSteps(userId);
            return Ok(status);
        }

        [Authorize]
        [HttpPost("report-user")]
        public async Task<IActionResult> ReportUser([FromBody] ReportUserDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var reporterId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                await _reportService.ReportUserAsync(reporterId, dto);
                return Ok(new { message = "User reported successfully. Our team will investigate." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while submitting the report." });
            }
        }
    }
}
