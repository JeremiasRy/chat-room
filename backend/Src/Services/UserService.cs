using backend.Src.Data;
using backend.Src.Models;
using System.Runtime.CompilerServices;

namespace backend.Src.Services;

public class UserService : IUserService
{
    private readonly IDb _db;
    public UserService(IDb db)
    {
        _db = db;
    }
    public async Task CreateUserAsync(string name)
    {
        await _db.SaveDataAsync<dynamic>(Db.DbSaveProcedures.CreateUser, new { p_name = name });
    }
    public async Task<ChatUser?> GetUserAsync(Guid? id = null, string? name = null)
    {
        var result = await _db.LoadDataAsync<ChatUser, dynamic>(Db.DbLoadProcedures.GetUser, new { p_id = id, p_name = name });
        return result.FirstOrDefault();
    }
    public async Task<List<string>> GetLoggedInUsersAsync()
    {
        var result = await _db.LoadDataAsync<string, dynamic>(Db.DbLoadProcedures.GetOnlineUsers, new { });
        return result.ToList();
    } 
    public async Task LoginUserAsync(Guid userId)
    {
        await _db.SaveDataAsync<dynamic>(Db.DbSaveProcedures.UserLogin, new { p_user_id = userId });
    }
}
