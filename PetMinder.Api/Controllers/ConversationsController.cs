using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetMinder.Data;
using PetMinder.Shared.DTO;
using System.Security.Claims;
namespace PetMinder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConversationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ConversationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private long GetUserId() => long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        [HttpGet]
        public async Task<ActionResult<List<ConversationDTO>>> GetUserConversations()
        {
            var userId = GetUserId();
            var conversations = await _context.Conversations
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Include(c => c.Messages)
                    .ThenInclude(m => m.Sender)
                .Where(c => c.Participants.Any(p => p.UserId == userId))
                .OrderByDescending(c => c.Messages.Max(m => (DateTime?)m.SentAt) ?? c.CreatedAt)
                .ToListAsync();

            var result = conversations.Select(c => new ConversationDTO
            {
                ConversationId = c.ConversationId,
                BookingId = c.BookingId,
                CreatedAt = c.CreatedAt,
                Participants = c.Participants.Select(p => new ConversationParticipantDTO
                {
                    ParticipantId = p.ParticipantId,
                    UserId = p.UserId,
                    UserName = p.User != null ? p.User.FirstName + " " + p.User.LastName : null,
                    JoinedAt = p.JoinedAt
                }).ToList(),
                LastMessage = c.Messages
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => new MessageDTO
                    {
                        MessageId = m.MessageId,
                        SenderId = m.SenderId,
                        SenderName = m.Sender != null ? m.Sender.FirstName + " " + m.Sender.LastName : null,
                        ConversationId = m.ConversationId,
                        Content = m.Content,
                        SentAt = m.SentAt
                    })
                    .FirstOrDefault()
            }).ToList();

            return Ok(result);
        }
    }
}
