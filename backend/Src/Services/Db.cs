using Dapper;
using Npgsql;

namespace backend.Src.Services;

public class Db : IDb
{
    private readonly IConfiguration _configuration;

    public Db(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IEnumerable<TModel>> CallDatabase<TModel, TParameters>(DbFunction procedure, TParameters parameters)
    {
        string connectionString = _configuration.GetConnectionString("Default");
        using var connection = new NpgsqlConnection(connectionString);
        return await connection.QueryAsync<TModel>(GetDbFunctionName(procedure), parameters, commandType: System.Data.CommandType.StoredProcedure);
    }
    static string GetDbFunctionName(DbFunction procedure)
    {
        return procedure switch
        {
            DbFunction.CreateUser => "create_user",
            DbFunction.CreateMessage => "create_message",
            DbFunction.UserLogin => "user_login",
            DbFunction.UserLogout => "user_logout",
            DbFunction.GetMessages => "get_paginated_messages_with_names",
            DbFunction.GetUser => "get_chat_user",
            DbFunction.GetOnlineUsers => "get_online_users",
            _ => throw new ArgumentException($"{nameof(procedure)}, was not a valid db function")
        };
    }

    public enum DbFunction
    {
        CreateUser,
        CreateMessage,
        UserLogin,
        UserLogout,
        GetMessages,
        GetUser,
        GetOnlineUsers
    }
}
