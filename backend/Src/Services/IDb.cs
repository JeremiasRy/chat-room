namespace backend.Src.Services;

public interface IDb
{
    Task<IEnumerable<TModel>> CallDatabase<TModel, TParameters>(Db.DbFunction procedure, TParameters parameters);
}