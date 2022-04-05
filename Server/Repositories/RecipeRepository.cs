using BlazingRecept.Server.Context;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazingRecept.Server.Repositories;

public class RecipeRepository : RepositoryBase<Recipe>, IRecipeRepository
{
    public RecipeRepository(BlazingReceptContext context) : base(context)
    {
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

    public override async Task<Recipe?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _context.Recipe
                .Include(recipe => recipe.IngredientMeasurements)
                .FirstOrDefaultAsync(recipe => recipe.Id == id);
        }
        catch (Exception)
        {
            return null;
        }
    }

    protected internal override async Task<Recipe?> GetDetachedByIdAsync(Guid id)
    {
        Recipe? detachedRecipe = await base.GetDetachedByIdAsync(id) ?? throw new InvalidOperationException();

        foreach (IngredientMeasurement ingredientMeasurement in detachedRecipe.IngredientMeasurements)
        {
            _context.Entry(ingredientMeasurement).State = EntityState.Detached;
        }

        return detachedRecipe;
    }

    public override async Task<IReadOnlyList<Recipe>?> ListAllAsync()
    {
        try
        {
            return await _context.Recipe
                .Include(recipe => recipe.IngredientMeasurements)
                .ToListAsync();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public override async Task<Recipe> AddAsync(Recipe recipe)
    {
        try
        {
            _context.Add(recipe);

            await _context.SaveChangesAsync();
            await _context.Entry(recipe).ReloadAsync();
        }
        catch (Exception)
        {
        }

        return recipe;
    }

    public override async Task<Recipe> UpdateAsync(Recipe recipe)
    {
        try
        {
            Recipe? oldRecipe = await GetDetachedByIdAsync(recipe.Id) ?? throw new InvalidOperationException();

            UpdateRecipeIngredientMeasurements(oldRecipe, recipe);

            _context.Entry(recipe).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            await _context.Entry(recipe).ReloadAsync();
        }
        catch (Exception)
        {
        }

        return recipe;
    }

    private void UpdateRecipeIngredientMeasurements(Recipe oldRecipe, Recipe newRecipe)
    {
        foreach (IngredientMeasurement ingredientMeasurementInNewRecipe in newRecipe.IngredientMeasurements)
        {
            ApplyAddedOrModifiedState(ingredientMeasurementInNewRecipe);
        }

        foreach (IngredientMeasurement ingredientMeasurementInOldRecipe in oldRecipe.IngredientMeasurements)
        {
            bool ingredientMeasurementNotPresent = newRecipe.IngredientMeasurements
            .Any(ingredientMeasurementInNewRecipe => ingredientMeasurementInNewRecipe.Id == ingredientMeasurementInOldRecipe.Id) == false;

            if (ingredientMeasurementNotPresent)
            {
                _context.Entry(ingredientMeasurementInOldRecipe).State = EntityState.Deleted;
            }
        }
    }
}
