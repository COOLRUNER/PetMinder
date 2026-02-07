using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PetMinder.Api.Hubs;
using PetMinder.Data;
using PetMinder.Models;
using PetMinder.Shared.DTO;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessagesController(ApplicationDbContext context, IHubContext<ChatHub> hubController)
        {
            _context = context;
            _hubContext = hubController;
        }

        [HttpPost]
        public async Task<IActionResult> PostMessage(MessageDTO messageDTO)
        {
            var message = new Message
            {
                SenderId = messageDTO.SenderId,
                ConversationId = messageDTO.ConversationId,
                Content = messageDTO.Content,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            messageDTO.MessageId = message.MessageId;
            messageDTO.SentAt = message.SentAt;
            await _hubContext.Clients.Group(messageDTO.ConversationId.ToString())
                .SendAsync("ReceiveMessage", messageDTO);
            return Ok(messageDTO);
        }

        [HttpGet]
        public async Task<ActionResult<List<MessageDTO>>> GetMessages(long conversationId)
        {
            var messages = await _context.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.SentAt)
                .Select(m => new MessageDTO
                {
                    MessageId = m.MessageId,
                    SenderId = m.SenderId,
                    SenderName = m.Sender != null ? m.Sender.FirstName + " " + m.Sender.LastName : null,
                    ConversationId = m.ConversationId,
                    Content = m.Content,
                    SentAt = m.SentAt
                })
                .ToListAsync();

            return Ok(messages);
        }
    }
}
