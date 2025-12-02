namespace NotesPlatformBackend.Models;

public class NoteFile
{
    public int Id { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public int NoteId { get; set; }

    public Note Note { get; set; } = null!;
}
