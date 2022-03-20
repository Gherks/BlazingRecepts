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

    public async Task<bool> AnyAsync(string name)
    {
        try
        {
            return await _context.Ingredient.AnyAsync(entity => entity.Name == name);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
 