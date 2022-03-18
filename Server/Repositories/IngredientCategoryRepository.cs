using Receptacle.Server.Context;
using Receptacle.Server.Entities;
using Receptacle.Server.Repositories.Interfaces;

namespace Receptacle.Server.Repositories;

public class IngredientCategoryRepository : RepositoryBase<IngredientCategory>, IIngredientCategoryRepository
{
    public IngredientCategoryRepository(ReceptacleContext dbContext) : base(dbContext)
    {
    }
}
