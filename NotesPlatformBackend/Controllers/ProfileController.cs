using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesPlatformBackend.Data;
using NotesPlatformBackend.DTOs;
using System.Security.Claims;

namespace NotesPlatformBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProfileController(AppDbContext db)
    {
        _db = db;
    }

    private int GetUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                  User.FindFirstValue(ClaimTypes.Name) ??
                  User.FindFirstValue("sub");

        if (sub == null)
            throw new Exception("No user id claim");

        return int.Parse(sub);
    }

    [HttpGet("me")]
    public async Task<ActionResult<ProfileResponse>> GetMe()
    {
        var userId = GetUserId();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return NotFound();

        return Ok(new ProfileResponse
        {
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Bio = user.Bio
        });
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(ProfileUpdateRequest request)
    {
        var userId = GetUserId();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return NotFound();

        user.FullName = request.FullName;
        user.Bio = request.Bio;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("me/notes")]
    public async Task<IActionResult> GetMyNotes()
    {
        var userId = GetUserId();
        var notes = await _db.Notes
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.UpdatedAt)
            .ToListAsync();

        return Ok(notes.Select(n => new
        {
            n.Id,
            n.Title,
            n.CreatedAt,
            n.UpdatedAt
        }));
    }
}
