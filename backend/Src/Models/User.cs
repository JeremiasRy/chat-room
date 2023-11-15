namespace backend.Src.Models;

public class User : BaseModel
{
    string Name { get; set; } = null!;
    bool LoggedIn { get; set; }
    public DateTime LastLoginTime { get; set; }
    public List<Message> Message { get; set; } = null!;
}
