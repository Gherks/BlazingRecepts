using BlazingRecept.Server.Context;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazingRecept.Server.Repositories;

public class RecipeRepository : RepositoryBase<Recipe>, IRecipeRepository
{
    private IIngredientCategoryRepository? _ingredientCategoryRepository;

    public RecipeRepository(BlazingReceptContext context, IIngredientCategoryRepository ingredientCategoryRepository) : base(context)
    {
        _ingredientCategoryRepository = ingredientCategoryRepository;
    }

    public async Task<bool> AnyAsync(string name)
    {
        try
        {
            return await _context.Recipe.AnyAsync(ingredient => ingredient.Name.ToLower() == name.ToLower());
        }
        catch (Exception)
        {
            return false;
        }
    }

    public override async Task<Recipe> AddAsync(Recipe recipe)
    {
        if (_ingredientCategoryRepository == null) throw new InvalidOperationException("Ingredient category repository cannot be used because it has not been set.");

        try
        {
            //IngredientCategory? ingredientCategory = await _ingredientCategoryRepository.GetByIdAsync(ingredient.IngredientCategoryId);

            //if (ingredientCategory == null) throw new InvalidOperationException("Ingredient category cannot be used because it could not be fetched from database.");

            //ingredient.IngredientCategory = ingredientCategory;
            //_context.Ingredient.Add(ingredient);

            await _context.SaveChangesAsync();
            await _context.Entry(recipe).ReloadAsync();
        }
        catch (Exception)
        {
        }

        return recipe;
    }
}
