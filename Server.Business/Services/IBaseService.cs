namespace Server.Business.Services;

public interface IBaseService<TEntity>
{
    Task<List<TEntity>> GetAll();
    Task<TEntity> GetById(int id);
    Task<bool> Create(TEntity entity);
    Task<bool>  Update(int id, TEntity entity);
    Task Delete(int id);
    bool Exists(int id);
}
