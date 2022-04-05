using BlazingRecept.Server.Context;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazingRecept.Server.Repositories;

public class IngredientRepository : RepositoryBase<Ingredient>, IIngredientRepository
{
    public IngredientRepository(BlazingReceptContext context) : base(context)
    {
    }

    public async Task<bool> AnyAsync(string name)
    {
        try
        {
            return await _context.Ingredient.AnyAsync(ingredient => ingredient.Name.ToLower() == name.ToLower());
        }
        catch (Exception)
        {
            return false;
        }
    }
}
