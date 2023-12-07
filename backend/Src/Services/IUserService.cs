using backend.Src.Models;

namespace backend.Src.Services
{
    public interface IUserService
    {
        Task CreateUserAsync(string name, string email);
        Task<List<string>> GetLoggedInUsersAsync();
        Task<ChatUser?> GetUserAsync(Guid? id = null, string? email = null);
        Task LoginUserAsync(Guid userId, string connectionId);
        Task LogoutUserAsync(Guid userId);
    }
}