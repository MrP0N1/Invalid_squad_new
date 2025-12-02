namespace NotesPlatformBackend.DTOs;

public class ProfileResponse
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? FullName { get; set; }
    public string? Bio { get; set; }
}

public class ProfileUpdateRequest
{
    public string? FullName { get; set; }
    public string? Bio { get; set; }
}
