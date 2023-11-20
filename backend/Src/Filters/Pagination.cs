namespace backend.Src.Filters;

public class Pagination
{
    public DateTime? LastCreatedAt { get; set; } = null;
    public int PageSize { get; set; } = 20;
}
