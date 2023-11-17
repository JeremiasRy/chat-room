using Dapper;
using Npgsql;

namespace backend.Src.Data;

public class Db : IDb
{
    private readonly IConfiguration _configuration;

    public Db(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    static Db()
    {
       DefaultTypeMap.MatchNamesWithUnderscores = true;
    }
    public async Task SaveDataAsync<TParameters>(DbSaveProcedures procedure, TParameters parameters)
    {
        string connectionString = _configuration.GetConnectionString("Default");
        using var connection = new NpgsqlConnection(connectionString);
        await connection.ExecuteAsync(GetDbSaveProcedureName(procedure), parameters, commandType: System.Data.CommandType.StoredProcedure);
    }
    public async Task<IEnumerable<TModel>> LoadDataAsync<TModel, TParameters>(DbLoadProcedures procedure, TParameters parameters)
    {
        string connectionString = _configuration.GetConnectionString("Default");
        using var connection = new NpgsqlConnection(connectionString);
        return await connection.QueryAsync<TModel>(GetDbLoadProcedureName(procedure), parameters, commandType: System.Data.CommandType.StoredProcedure);
    }
    static string GetDbLoadProcedureName(DbLoadProcedures procedure)
    {
        return procedure switch
        {
            DbLoadProcedures.GetMessages => "get_paginated_messages_with_names",
            DbLoadProcedures.GetUser => "get_chat_user",
            DbLoadProcedures.GetOnlineUsers => "get_online_users",
            _ => throw new ArgumentException($"{nameof(procedure)}, was not a valid db function")
        };
    }
    static string GetDbSaveProcedureName(DbSaveProcedures procedure)
    {
        return procedure switch
        {
            DbSaveProcedures.CreateUser => "create_user",
            DbSaveProcedures.CreateMessage => "create_message",
            DbSaveProcedures.UserLogin => "user_login",
            DbSaveProcedures.UserLogout => "user_logout",
            _ => throw new ArgumentException($"{nameof(procedure)}, was not valid db function")
        };
    }
    public enum DbSaveProcedures
    {
        CreateUser,
        CreateMessage,
        UserLogin,
        UserLogout,
    }
    public enum DbLoadProcedures
    {
        GetMessages,
        GetUser,
        GetOnlineUsers,
    }
}
