using LetsTalk.DTOs.AppUsers;
using LetsTalk.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class AppUsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AppUsersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AppUserResponse>> GetUserById(Guid id)
        {
            var user = await _db.AppUsers
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new AppUserResponse(
                    u.Id,
                    u.UserName,
                    u.CreatedAt,
                    u.LastSeenAt))
                .FirstOrDefaultAsync();

            if (user is null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet]
        public async Task<ActionResult<List<AppUserResponse>>> GetUsers()
        {
            var users = await _db.AppUsers
                .AsNoTracking()
                .OrderBy(u => u.UserName)
                .Select(u => new AppUserResponse(
                    u.Id,
                    u.UserName,
                    u.CreatedAt,
                    u.LastSeenAt))
                .ToListAsync();

            return Ok(users);
        }


    }
}
