using BlazingRecept.Logging;
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
        catch (Exception exception)
        {
            Log.Error(exception, $"Repository failed check existence of ingredient with name: {name}");
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
            Log.Error(exception, $"Repository failed to fetch ingredient with id: {id}");
            return null;
        }
    }

    public async Task<Ingredient?> GetByNameAsync(string name)
    {
        try
        {
            return await _context.Set<Ingredient>()
                .Include(ingredient => ingredient.Category)
                .FirstAsync(ingredient => ingredient.Name.ToLower() == name.ToLower());
        }
        catch (Exception exception)
        {
            Log.Error(exception, $"Repository failed to fetch recipe with name: {name}");
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
            Log.Error(exception, "Repository failed to fetch many ingredients");
            return null;
        }
    }

    public override async Task<Ingredient?> AddAsync(Ingredient ingredient)
    {
        try
        {
            ingredient.Category = await _context.Category
                .Where(category => category.Id == ingredient.CategoryId)
                .FirstAsync();

            _context.Set<Ingredient>().Add(ingredient);

            await _context.SaveChangesAsync();
            await _context.Entry(ingredient).ReloadAsync();
        }
        catch (Exception exception)
        {
            Log.Error(exception, $"Repository failed to add ingredient: {ingredient}");
            return null;
        }

        return ingredient;
    }
}
