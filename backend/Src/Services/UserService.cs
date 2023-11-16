using backend.Src.Models;

namespace backend.Src.Services;

public class UserService : IUserService
{
    private readonly IDb _db;
    public UserService(IDb db)
    {
        _db = db;
    }
    public async Task<Guid?> CreateUser(string name)
    {
        var result = await _db.CallDatabase<Guid?, dynamic>(Db.DbFunction.CreateUser, new { p_name = name });
        return result.FirstOrDefault();
    }
    public async Task<ChatUser?> GetUser(Guid id)
    {
        var result = await _db.CallDatabase<ChatUser?, dynamic>(Db.DbFunction.GetUser, new { p_id = id });
        return result.FirstOrDefault();
    }
}
