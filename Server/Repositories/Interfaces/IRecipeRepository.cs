using BlazingRecept.Server.Entities;

namespace BlazingRecept.Server.Repositories.Interfaces;

public interface IRecipeRepository : IAsyncRepository<Recipe>
{
    Task<bool> AnyAsync(string name);
}
