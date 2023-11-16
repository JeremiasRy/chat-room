namespace backend.Src.Models;

public class Message : BaseModel
{
    public string Content { get; set; } = null!;
    public int UserId { get; set; }
}
