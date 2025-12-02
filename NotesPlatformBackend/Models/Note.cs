namespace NotesPlatformBackend.Models;

public class Note
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public ICollection<NoteFile> Files { get; set; } = new List<NoteFile>();
}
