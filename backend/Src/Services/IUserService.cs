using backend.Src.Models;

namespace backend.Src.Services;

public interface IUserService
{
    Task<Guid?> CreateUser(string name);
    Task<ChatUser?> GetUser(Guid id);
}