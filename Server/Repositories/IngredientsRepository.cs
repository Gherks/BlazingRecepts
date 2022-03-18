using BlazingRecept.Server.Context;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazingRecept.Server.Repositories;

public class IngredientsRepository : RepositoryBase<Ingredient>, IIngredientsRepository
{
    public IngredientsRepository(BlazingReceptContext dbContext) : base(dbContext)
    {
    }

    public override async Task<IReadOnlyList<Ingredient>> ListAllAsync()
    {
        return await _dbContext.Set<Ingredient>().ToListAsync();
    }
}
