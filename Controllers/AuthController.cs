using LetsTalk.Domain.Entities;
using LetsTalk.DTOs.AppUsers;
using LetsTalk.DTOs.Auth;
using LetsTalk.Persistence;
using LetsTalk.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<AppUser> _passwordHasher;

        public AuthController(AppDbContext db, ITokenService tokenService, IPasswordHasher<AppUser> passwordHasher)
        {
            _db = db;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(
            [FromBody] RegisterRequest request,
            CancellationToken ct)
        {
            var normalisedUserName = request.UserName!.Trim();
            var lowerUserName = normalisedUserName.ToLower();
            var normalisedEmail = request.Email!.Trim().ToLowerInvariant();

            var alreadyExist = await _db.AppUsers
                .AsNoTracking()
                .Where(u => u.UserName.ToLower() == lowerUserName || u.Email.ToLower() == normalisedEmail)
                .Select(u => new { UserName = u.UserName.ToLower(), Email = u.Email.ToLower() })
                .FirstOrDefaultAsync(ct);

            if (alreadyExist is not null)
            {
                if (alreadyExist.UserName == lowerUserName)
                {
                    return Conflict(new { message = "Username is already registered." });
                }
                else
                {
                    return Conflict(new { message = "Email is already registered" });
                }
            }

            var guid = Guid.NewGuid();
            var now = DateTimeOffset.UtcNow;

            var newUser = new AppUser
            {
                Id = guid,
                UserName = normalisedUserName,
                Email = normalisedEmail,
                PasswordHash = string.Empty,
                CreatedAt = now,
            };

            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, request.Password!);

            _db.AppUsers.Add(newUser);

            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = "The user already exists." });
            }

            var tokenResponse = _tokenService.GenerateToken(newUser);
            var userResponse = new AppUserResponse(
                newUser.Id,
                newUser.UserName,
                newUser.CreatedAt,
                newUser.LastSeenAt
                );

            var response = new AuthResponse(tokenResponse, userResponse);

            return CreatedAtAction(
                actionName: nameof(AppUsersController.GetUserById),
                controllerName: "AppUsers",
                routeValues: new { id = newUser.Id },
                value: response);
        }

    }
}