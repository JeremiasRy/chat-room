using backend.Src.DTOs;
using backend.Src.Filters;

namespace backend.Src.Services
{
    public interface IMessageService
    {
        Task<bool> CreateMessageAsync(MessageDTO message);
        Task<List<DisplayMessageDTO>> GetMessagesAsync(Pagination? pagination);
    }
}