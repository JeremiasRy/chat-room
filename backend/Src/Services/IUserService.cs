using backend.Src.Models;

namespace backend.Src.Services;

public interface IUserService
{
    Task CreateUserAsync(string name);
    Task<ChatUser?> GetUserAsync(Guid? id = null, string? name = null);
}