using BlazingRecept.Server.Context;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazingRecept.Server.Repositories;

public class IngredientsRepository : RepositoryBase<Ingredient>, IIngredientsRepository
{
    private IIngredientCategoryRepository? _ingredientCategoryRepository;

    public IngredientsRepository(BlazingReceptContext context, IIngredientCategoryRepository ingredientCategoryRepository) : base(context)
    {
        _ingredientCategoryRepository = ingredientCategoryRepository;
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

    public override async Task<Ingredient> AddAsync(Ingredient ingredient)
    {
        if (_ingredientCategoryRepository == null) throw new InvalidOperationException("Ingredient category repository cannot be used because it has not been set.");

        try
        {
            IngredientCategory? ingredientCategory = await _ingredientCategoryRepository.GetByIdAsync(ingredient.IngredientCategoryId);

            if (ingredientCategory == null) throw new InvalidOperationException("Ingredient category cannot be used because it could not be fetched from database.");

            ingredient.IngredientCategory = ingredientCategory;
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
