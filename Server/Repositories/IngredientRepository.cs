using BlazingRecept.Server.Context;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BlazingRecept.Server.Repositories;

public class IngredientRepository : RepositoryBase<Ingredient>, IIngredientRepository
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "IngredientRepository";

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
            Log.ForContext(_logProperty, _logDomainName).Error(exception, "Repository failed check existence of ingredient with name: {@Name}", name);
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
            const string errorMessage = "Repository failed to fetch entity with id: {@Id}";
            Log.ForContext(_logProperty, _logDomainName).Error(exception, errorMessage, id);

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
            const string errorMessage = "Repository failed to fetch recipe with name: {@Name}";
            Log.ForContext(_logProperty, _logDomainName).Error(exception, errorMessage, name);

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
            const string errorMessage = "Repository failed to fetch many entities";
            Log.ForContext(_logProperty, _logDomainName).Error(exception, errorMessage);

            return null;
        }
    }
}
