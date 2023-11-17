using static backend.Src.Data.Db;

namespace backend.Src.Data;

public interface IDb
{
    Task<IEnumerable<TModel>> LoadDataAsync<TModel, TParameters>(DbLoadProcedures procedure, TParameters parameters);
    Task SaveDataAsync<TParameters>(DbSaveProcedures procedure, TParameters parameters);
}