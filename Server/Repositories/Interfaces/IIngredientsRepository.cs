using BlazingRecept.Server.Entities;

namespace BlazingRecept.Server.Repositories.Interfaces;

public interface IIngredientsRepository : IAsyncRepository<Ingredient>
{
    Task<bool> AnyAsync(string name);
}
