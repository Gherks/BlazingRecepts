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

    public override async Task<IReadOnlyList<Ingredient>?> ListAllAsync()
    {
        try
        {
            return await _context.Ingredient
                .Include(ingredient => ingredient.Category)
                .ToListAsync();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public override async Task<Ingredient> AddAsync(Ingredient ingredient)
    {
        try
        {
            _context.Attach(ingredient.Category);
            _context.Ingredient.Add(ingredient);

            await _context.SaveChangesAsync();
            await _context.Entry(ingredient).ReloadAsync();
        }
        catch (Exception)
        {
        }

        return ingredient;
    }
}
