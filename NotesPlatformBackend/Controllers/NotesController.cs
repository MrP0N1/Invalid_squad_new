using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesPlatformBackend.Data;
using NotesPlatformBackend.DTOs;
using NotesPlatformBackend.Models;
using System.Security.Claims;

namespace NotesPlatformBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public NotesController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NoteResponse>>> GetMyNotes()
    {
        var userId = GetUserId();
        var notes = await _db.Notes
            .Where(n => n.UserId == userId)
            .Include(n => n.Files)
            .OrderByDescending(n => n.UpdatedAt)
            .ToListAsync();

        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var result = notes.Select(n => new NoteResponse
        {
            Id = n.Id,
            Title = n.Title,
            Content = n.Content,
            CreatedAt = n.CreatedAt,
            UpdatedAt = n.UpdatedAt,
            Files = n.Files.Select(f => new FileResponse
            {
                Id = f.Id,
                FileName = f.FileName,
                Url = $"{baseUrl}/{f.FilePath.Replace("\\", "/")}"
            }).ToList()
        });

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<NoteResponse>> Create(NoteCreateRequest request)
    {
        var userId = GetUserId();

        var note = new Note
        {
            Title = request.Title,
            Content = request.Content,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Notes.Add(note);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMyNotes), new { id = note.Id }, new NoteResponse
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, NoteUpdateRequest request)
    {
        var userId = GetUserId();
        var note = await _db.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
        if (note == null) return NotFound();

        note.Title = request.Title;
        note.Content = request.Content;
        note.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        var note = await _db.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
        if (note == null) return NotFound();

        _db.Notes.Remove(note);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:int}/files")]
    public async Task<ActionResult<FileResponse>> UploadFile(int id, IFormFile file)
    {
        var userId = GetUserId();
        var note = await _db.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
        if (note == null) return NotFound();

        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Файл пустой" });

        var uploadsDir = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
        Directory.CreateDirectory(uploadsDir);

        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadsDir, uniqueFileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var relativePath = Path.Combine("uploads", uniqueFileName).Replace("\\", "/");

        var noteFile = new NoteFile
        {
            FileName = file.FileName,
            FilePath = relativePath,
            NoteId = note.Id
        };

        _db.NoteFiles.Add(noteFile);
        await _db.SaveChangesAsync();

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var response = new FileResponse
        {
            Id = noteFile.Id,
            FileName = noteFile.FileName,
            Url = $"{baseUrl}/{relativePath}"
        };

        return Ok(response);
    }
}
