namespace backend.Src.Models;

public class ChatUser : BaseModel
{
    public string Name { get; set; } = null!;
    public bool Online { get; set; }
    public DateTime LastLoginTime { get; set; }
    public List<Message>? Messages { get; set; }
}
