namespace NotesPlatformBackend.DTOs;

public class NoteCreateRequest
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
}

public class NoteUpdateRequest
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
}

public class NoteResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<FileResponse> Files { get; set; } = new();
}

public class FileResponse
{
    public int Id { get; set; }
    public string FileName { get; set; } = null!;
    public string Url { get; set; } = null!;
}
