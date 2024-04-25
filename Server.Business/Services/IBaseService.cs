namespace Server.Business.Services;

public interface IBaseService<TEntity, TDto, TDbDto>
{
    Task<List<TEntity>> GetAll();
    Task<TEntity?> GetById(string id);
    Task<bool>  Update(string id, TDto dto);
    Task Delete(string id);
    Task<bool>  Exists(string id);
}
