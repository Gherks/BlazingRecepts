using Receptacle.Server.Context;
using Receptacle.Server.Entities;
using Receptacle.Server.Repositories.Interfaces;

namespace Receptacle.Server.Repositories;

public class RecipesRepository : RepositoryBase<Recipe>, IRecipesRepository
{
    public RecipesRepository(ReceptacleContext dbContext) : base(dbContext)
    {
    }
}
