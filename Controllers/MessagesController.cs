using LetsTalk.Domain.Entities;
using LetsTalk.Domain.Enums;
using LetsTalk.DTOs.Messages;
using LetsTalk.Extensions;
using LetsTalk.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Controllers
{
    [ApiController]
    [Route("api/conversations/{conversationId:guid}/messages")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public MessagesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<ActionResult<MessageResponse>> SendMessage(
            [FromRoute] Guid conversationId,
            [FromBody] SendMessageRequest request)
        {
            var userIdClaim = User.GetUserId();
            if (userIdClaim is null)
            {
                return Unauthorized();
            }

            var senderId = userIdClaim.Value;
            var content = request.Content!.Trim();

            if (conversationId == Guid.Empty)
            {
                return BadRequest(new { message = "Valid conversationId is required." });
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest(new { message = "Message content cannot be empty." });
            }

            var conversation = await _db.Conversations
                .Include(c => c.Participants)
                .ThenInclude(cp => cp.User)
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation is null)
            {
                return NotFound(new { message = "Conversation not found"});
            }

            var senderParticipant = conversation.Participants
                .FirstOrDefault(cp => cp.UserId == senderId);

            if (senderParticipant is null)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { message = "Sender is not a participant in this conversation" });
            }

            var now = DateTimeOffset.UtcNow;

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                SenderId = senderId,
                Content = content,
                Status = MessageStatus.Sent,
                SentAt = now
            };

            conversation.LastMessageAt = now;
            conversation.LastMessagePreview = content.Length <= 100 ? content : content.Substring(0, 100);
            _db.Messages.Add(message);

            await _db.SaveChangesAsync();

            var response = new MessageResponse(
                message.Id,
                conversationId,
                senderId,
                senderParticipant.User.UserName,
                content,
                message.Status,
                message.SentAt,
                message.EditedAt
                );

            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpGet]
        public async Task<ActionResult<List<MessageResponse>>> GetMessages(Guid conversationId)
        {
            var userIdClaim = User.GetUserId();
            if (userIdClaim is null)
            {
                return Unauthorized();
            }

            var userId = userIdClaim.Value;

            if (conversationId == Guid.Empty)
            {
                return BadRequest(new { message = "Valid conversationId is required."});
            }

            var conversationExists = await _db.Conversations
                .AsNoTracking()
                .AnyAsync(c => c.Id == conversationId);

            if (!conversationExists)
            {
                return NotFound(new { message = "Conversation not found." });
            }

            var isParticipant = await _db.ConversationParticipants
                .AsNoTracking()
                .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (!isParticipant)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { message = "User is not a participant in this conversation." });
            }

            var messages = await _db.Messages
                .AsNoTracking()
                .Where(m => m.ConversationId == conversationId)
                .Select(m => new MessageResponse(
                    m.Id,
                    m.ConversationId,
                    m.SenderId,
                    m.Sender.UserName,
                    m.Content,
                    m.Status,
                    m.SentAt,
                    m.EditedAt))
                .ToListAsync();

            var ordered = messages.OrderBy(m => m.SentAt).ToList();

            return Ok(ordered);

        }
    }
}
