using BlazingRecept.Server.Entities;

namespace BlazingRecept.Server.Repositories.Interfaces;

public interface IIngredientRepository : IAsyncRepository<Ingredient>
{
    Task<bool> AnyAsync(string name);
}
