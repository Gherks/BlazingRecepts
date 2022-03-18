using BlazingRecept.Server.Entities.Base;

namespace BlazingRecept.Server.Repositories.Interfaces;

public interface IAsyncRepository<Type> where Type : BaseEntity
{
    Task<Type?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Type>> ListAllAsync();
    Task<Type> AddAsync(Type entity);
    Task<IEnumerable<Type>> AddManyAsync(IEnumerable<Type> entities);
    Task<Type> UpdateAsync(Type entity);
    Task UpdateManyAsync(IEnumerable<Type> entities);
    Task<bool> DeleteAsync(Type entity);
    Task<bool> DeleteManyAsync(IEnumerable<Type> entities);
}
