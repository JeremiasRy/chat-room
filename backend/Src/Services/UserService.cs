using backend.Src.Data;
using backend.Src.Models;

namespace backend.Src.Services;

public class UserService : IUserService
{
    private readonly IDb _db;
    public UserService(IDb db)
    {
        _db = db;
    }
    public async Task CreateUserAsync(string name, string email)
    {
        await _db.SaveDataAsync<dynamic>(Db.DbSaveProcedures.CreateUser, new { p_name = name, p_email = email });
    }
    public async Task<ChatUser?> GetUserAsync(Guid? id = null, string? email = null)
    {
        var result = await _db.LoadDataAsync<ChatUser, dynamic>(Db.DbLoadProcedures.GetUser, new { p_id = id, p_email = email });
        return result.FirstOrDefault();
    }
    public async Task<List<string>> GetLoggedInUsersAsync()
    {
        var result = await _db.LoadDataAsync<string, dynamic>(Db.DbLoadProcedures.GetOnlineUsers, new { });
        return result.ToList();
    }
    public async Task LoginUserAsync(Guid userId, string connectionId)
    {
        await _db.SaveDataAsync<dynamic>(Db.DbSaveProcedures.UserLogin, new { p_user_id = userId, p_connection_id = connectionId });
    }
    public async Task LogoutUserAsync(Guid userId)
    {
        await _db.SaveDataAsync<dynamic>(Db.DbSaveProcedures.UserLogout, new { p_user_id = userId });
    }
}
