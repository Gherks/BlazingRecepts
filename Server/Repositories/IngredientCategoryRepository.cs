using BlazingRecept.Server.Context;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;

namespace BlazingRecept.Server.Repositories;

public class IngredientCategoryRepository : RepositoryBase<IngredientCategory>, IIngredientCategoryRepository
{
    public IngredientCategoryRepository(BlazingReceptContext dbContext) : base(dbContext)
    {
    }
}
