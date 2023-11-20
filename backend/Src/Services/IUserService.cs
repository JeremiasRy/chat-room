using backend.Src.Models;

namespace backend.Src.Services
{
    public interface IUserService
    {
        Task CreateUserAsync(string name);
        Task<List<string>> GetLoggedInUsersAsync();
        Task<ChatUser?> GetUserAsync(Guid? id = null, string? name = null);
        Task LoginUserAsync(Guid userId, string connectionId);
        Task LogoutUserAsync(Guid userId);
    }
}