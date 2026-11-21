using LetsTalk.Domain.Entities;
using LetsTalk.DTOs.ConversationParticipants;
using LetsTalk.DTOs.Conversations;
using LetsTalk.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LetsTalk.Extensions;

namespace LetsTalk.Controllers
{
    [ApiController]
    [Route("api/conversations")]
    [Authorize]
    public class ConversationsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ConversationsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<ActionResult<ConversationResponse>> CreateConversation(CreateConversationRequest request)
        {
            var senderIdClaim = User.GetUserId();
            if (senderIdClaim is null)
            {
                return Unauthorized();
            }

            var senderId = senderIdClaim.Value;
            var connectToId = request.ConnectToId!.Value;

            if (connectToId == Guid.Empty || senderId == connectToId)
            {
                return BadRequest(new { message = "Two distinct, valid user IDs are required." });
            }

            var users = await _db.AppUsers
                .AsNoTracking()
                .Where(u => u.Id == senderId || u.Id == connectToId)
                .Select(u => new { u.Id, u.UserName })
                .ToListAsync();

            if (users.Count != 2)
            {
                return NotFound(new { message = "The target user not found." });
            }

            var participantsResponse = users
                .Select(u => new ConversationParticipantResponse(u.Id, u.UserName))
                .ToList();

            var senderIdStr = senderId.ToString("D");
            var connectToIdStr = connectToId.ToString("D");

            string directParticipantKey;

            if (string.CompareOrdinal(senderIdStr, connectToIdStr) < 0)
            {
                directParticipantKey = $"{senderIdStr}_{connectToIdStr}";
            }
            else
            {
                directParticipantKey = $"{connectToIdStr}_{senderIdStr}";
            }

            var existedConversation = await _db.Conversations
                .AsNoTracking()
                .Where(c => c.DirectParticipantKey == directParticipantKey)
                .Select(c => new ConversationResponse(
                    c.Id,
                    c.CreatedAt,
                    c.LastMessageAt,
                    c.LastMessagePreview,
                    c.Participants
                        .Select(cp => new ConversationParticipantResponse(cp.UserId, cp.User.UserName)).ToList()))
                .FirstOrDefaultAsync();

            if (existedConversation is not null)
            {
                return Ok(existedConversation);
            }

            var now = DateTimeOffset.UtcNow;
            var conversationId = Guid.NewGuid();

            var conversation = new Conversation
            {
                Id = conversationId,
                DirectParticipantKey = directParticipantKey,
                CreatedAt = now,
                Participants = new List<ConversationParticipant>
                {
                    new ConversationParticipant
                    {
                        ConversationId = conversationId,
                        UserId = senderId,
                        JoinedAt = now
                    },
                    new ConversationParticipant
                    {
                        ConversationId = conversationId,
                        UserId = connectToId,
                        JoinedAt = now
                    }
                }
            };

            _db.Conversations.Add(conversation);
            await _db.SaveChangesAsync();

            var response = new ConversationResponse(
                conversation.Id,
                conversation.CreatedAt,
                conversation.LastMessageAt,
                conversation.LastMessagePreview,
                participantsResponse);

            return CreatedAtAction(nameof(GetConversationById), new { id = conversation.Id }, response);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ConversationResponse>> GetConversationById(Guid id)
        {
            var currentUserIdClaim = User.GetUserId();
            if (currentUserIdClaim is null)
            {
                return Unauthorized();
            }

            var currentUserId = currentUserIdClaim.Value;

            var response = await _db.Conversations
                .AsNoTracking()
                .Where(c => c.Id == id && c.Participants.Any(cp => cp.UserId == currentUserId))
                .Select(c => new ConversationResponse(
                    c.Id,
                    c.CreatedAt,
                    c.LastMessageAt,
                    c.LastMessagePreview,
                    c.Participants
                        .Select(cp => new ConversationParticipantResponse(
                            cp.UserId, 
                            cp.User.UserName))
                        .ToList()))
                .FirstOrDefaultAsync();

            if (response is null)
            {
                return NotFound();
            }

            return Ok(response);
        }
        [HttpGet]
        public async Task<ActionResult<List<ConversationResponse>>> GetUserConversations ()
        {
            var userIdClaim = User.GetUserId();
            if (userIdClaim is null)
            {
                return Unauthorized();
            }

            var userId = userIdClaim.Value;

            var conversations = await _db.Conversations
                .AsNoTracking()
                .Where(c => c.Participants.Any(cp => cp.UserId == userId))
                .Select(c => new ConversationResponse(
                    c.Id,
                    c.CreatedAt,
                    c.LastMessageAt,
                    c.LastMessagePreview,
                    c.Participants
                        .Select(cp => new ConversationParticipantResponse(
                            cp.UserId,
                            cp.User.UserName))
                        .ToList()))
                .ToListAsync();

            var ordered = conversations
               .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
               .ToList();

            return Ok(ordered);

        }
        
    }
}
