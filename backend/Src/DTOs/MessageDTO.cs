namespace backend.Src.DTOs;

public class MessageDTO
{
    public string Content { get; set; } = null!;
    public Guid UserId { get; set; }
}
