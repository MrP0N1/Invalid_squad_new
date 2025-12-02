namespace NotesPlatformBackend.Models;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Bio { get; set; }

    public ICollection<Note> Notes { get; set; } = new List<Note>();
}
