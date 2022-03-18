using BlazingRecept.Server.Context;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;

namespace BlazingRecept.Server.Repositories;

public class RecipesRepository : RepositoryBase<Recipe>, IRecipesRepository
{
    public RecipesRepository(BlazingReceptContext dbContext) : base(dbContext)
    {
    }
}
