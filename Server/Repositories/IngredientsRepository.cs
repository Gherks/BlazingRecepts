using Microsoft.EntityFrameworkCore;
using Receptacle.Server.Context;
using Receptacle.Server.Entities;
using Receptacle.Server.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Receptacle.Server.Repositories;

public class IngredientsRepository : RepositoryBase<Ingredient>, IIngredientsRepository
{
    public IngredientsRepository(ReceptacleContext dbContext) : base(dbContext)
    {
    }

    public override async Task<IReadOnlyList<Ingredient>> ListAllAsync()
    {
        return await _dbContext.Set<Ingredient>().ToListAsync();
    }
}
