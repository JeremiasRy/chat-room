using backend.Src.Data;
using backend.Src.DTOs;
using backend.Src.Filters;

namespace backend.Src.Services;

public class MessageService : IMessageService
{
    private readonly IDb _db;
    public MessageService(IDb db)
    {
        _db = db;
    }

    public async Task<List<DisplayMessageDTO>> GetMessagesAsync(Pagination? pagination)
    {
        pagination ??= new Pagination();
        var result = await _db.LoadDataAsync<DisplayMessageDTO, dynamic>(Db.DbLoadProcedures.GetMessages, new { p_last_created_at = pagination.LastCreatedAt, p_page_size = pagination.PageSize });
        return result.Reverse().ToList();
    }

    public async Task<bool> CreateMessageAsync(MessageDTO message)
    {
        try
        {
            await _db.SaveDataAsync(Db.DbSaveProcedures.CreateMessage, new { p_user_id = message.UserId, p_content = message.Content });
            return true;
        } catch
        {
            return false;
        }
        
    }
}
