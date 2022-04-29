using BlazingRecept.Server.Context;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
        catch (Exception exception)
        {
            Log.Error(exception, "Repository failed check existence of ingredient with name: {@Name}", name);
            return false;
        }
    }

    public override async Task<Ingredient?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _context.Set<Ingredient>()
                .Include(ingredient => ingredient.Category)
                .FirstOrDefaultAsync(ingredient => ingredient.Id == id);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Repository failed to fetch entity with id: {@Id}", id);
            return null;
        }
    }

    public override async Task<IReadOnlyList<Ingredient>?> ListAllAsync()
    {
        try
        {
            return await _context.Set<Ingredient>()
                .Include(ingredient => ingredient.Category)
                .ToListAsync();
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Repository failed to fetch many entities");
            return null;
        }
    }
}
