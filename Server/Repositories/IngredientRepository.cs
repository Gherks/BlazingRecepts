using BlazingRecept.Server.Context;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BlazingRecept.Server.Repositories;

public class IngredientRepository : RepositoryBase<Ingredient>, IIngredientRepository
{
    private static readonly string _logProperty = "Domain";

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
            const string messageTemplate = "Repository failed check existence of ingredient with name: {@Name}";
            Log.ForContext(_logProperty, GetType().Name).Error(exception, messageTemplate, name);

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
            const string messageTemplate = "Repository failed to fetch ingredient with id: {@Id}";
            Log.ForContext(_logProperty, GetType().Name).Error(exception, messageTemplate, id);

            return null;
        }
    }

    public async Task<Ingredient?> GetByNameAsync(string name)
    {
        try
        {
            return await _context.Set<Ingredient>().FirstAsync(ingredient => ingredient.Name.ToLower() == name.ToLower());
        }
        catch (Exception exception)
        {
            const string messageTemplate = "Repository failed to fetch recipe with name: {@Name}";
            Log.ForContext(_logProperty, GetType().Name).Error(exception, messageTemplate, name);

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
            const string errorMessage = "Repository failed to fetch many ingredients";
            Log.ForContext(_logProperty, GetType().Name).Error(exception, errorMessage);

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
            const string messageTemplate = "Repository failed to add ingredient: {@Ingredient}";
            Log.ForContext(_logProperty, GetType().Name).Error(exception, messageTemplate, ingredient);

            return null;
        }

        return ingredient;
    }
}
