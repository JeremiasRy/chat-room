namespace backend.Src.Filters;

public class Pagination
{
    DateTime? LastCreatedBy { get; set; } = null;
    public int PageSize { get; set; } = 20;
}
