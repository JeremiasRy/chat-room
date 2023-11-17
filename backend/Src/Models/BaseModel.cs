namespace backend.Src.Models;

public abstract class BaseModel
{
    public Guid Id { get; set; }
    public DateTime Created_At { get; set; }
    public DateTime Updated_At { get; set; }
}
