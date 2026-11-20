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

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(
            [FromBody] LoginRequest request,
            CancellationToken ct)
        {
            var normalisedEmail = request.Email!.Trim().ToLowerInvariant();
            var password = request.Password!;

            var user = await _db.AppUsers
                .FirstOrDefaultAsync(u => u.Email.ToLower() ==  normalisedEmail, ct);

            if (user is null)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            PasswordVerificationResult passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            } else if (passwordResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, password);
            }

            var now = DateTimeOffset.UtcNow;
            user.LastSeenAt = now;

            await _db.SaveChangesAsync(ct);

            var tokenResponse = _tokenService.GenerateToken(user);
            var userResponse = new AppUserResponse(
                user.Id,
                user.UserName,
                user.CreatedAt,
                user.LastSeenAt
                );
            var response = new AuthResponse(tokenResponse, userResponse);

            return Ok(response);



        }

    }
}