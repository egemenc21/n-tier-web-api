namespace Server.Business.Services;

public interface IBaseService<TEntity>
{
    Task<List<TEntity>> GetAll();
    Task<TEntity?> GetById(string id);
    Task<bool>  Update(string id, TEntity entity);
    Task Delete(string id);
    Task<bool>  Exists(string id);
}
