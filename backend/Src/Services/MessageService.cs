using backend.Src.Data;
using backend.Src.DTOs;
using backend.Src.Filters;

namespace backend.Src.Services;

public class MessageService
{
    private readonly IDb _db;
    public MessageService(IDb db)
    {
        _db = db;
    }

    public async Task<List<DisplayMessageDTO>> GetMessagesAsync(Pagination? pagination)
    {
        pagination ??= new Pagination();
        var result = await _db.LoadDataAsync<DisplayMessageDTO, Pagination>(Db.DbLoadProcedures.GetMessages, pagination);
        return result.ToList();
    }

    public async Task CreateMessageAsync(MessageDTO message)
    {
    }
}
