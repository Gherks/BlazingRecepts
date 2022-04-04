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
                .Include(recipe => recipe.Category)
                .Include(recipe => recipe.IngredientMeasurements)
                    .ThenInclude(ingredientMeasurement => ingredientMeasurement.Ingredient)
                        .ThenInclude(ingredient => ingredient.Category)
                .FirstOrDefaultAsync(recipe => recipe.Id == id);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public override async Task<IReadOnlyList<Recipe>?> ListAllAsync()
    {
        try
        {
            return await _context.Recipe
                .Include(recipe => recipe.Category)
                .Include(recipe => recipe.IngredientMeasurements)
                    .ThenInclude(ingredientMeasurement => ingredientMeasurement.Ingredient)
                        .ThenInclude(ingredient => ingredient.Category)
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
            foreach (IngredientMeasurement ingredientMeasurement in recipe.IngredientMeasurements)
            {
                bool isTrackingIngredient = _context.ChangeTracker.Entries<Ingredient>().Any(entry => entry.Entity.Id == ingredientMeasurement.Ingredient.Id);
                bool isTrackingIngredientCategory = _context.ChangeTracker.Entries<Category>().Any(entry => entry.Entity.Id == ingredientMeasurement.Ingredient.Category.Id);

                if (isTrackingIngredient == false)
                {
                    _context.Attach(ingredientMeasurement.Ingredient);
                }

                if (isTrackingIngredientCategory == false)
                {
                    _context.Attach(ingredientMeasurement.Ingredient.Category);
                }
            }



            _context.Attach(recipe.Category);
            _context.Recipe.Add(recipe);

            await _context.SaveChangesAsync();
            await _context.Entry(recipe).ReloadAsync();
        }
        catch (Exception exc)
        {
            exc = exc;
        }

        return recipe;
    }

    public override async Task<Recipe> UpdateAsync(Recipe recipe)
    {
        try
        {
            Recipe? oldRecipe = await GetByIdAsync(recipe.Id);

            if (oldRecipe == null) throw new InvalidOperationException("Failed to fetch old version of recipe when trying to update it.");

            UpdateRecipeIngredientMeasurements(oldRecipe, recipe);

            _context.Entry(recipe.Category).State = EntityState.Modified;
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
        foreach (IngredientMeasurement ingredientMeasurement in newRecipe.IngredientMeasurements)
        {
            ApplyAddedOrModifiedState(ingredientMeasurement.Ingredient);
            ApplyAddedOrModifiedState(ingredientMeasurement.Ingredient.Category);
        }

        foreach (IngredientMeasurement oldIngredientMeasurement in oldRecipe.IngredientMeasurements)
        {
            bool oldIngredientMeasurementHasBeenRemoved = newRecipe.IngredientMeasurements
                .Any(ingredientMeasurement => ingredientMeasurement.Id == oldIngredientMeasurement.Id) == false;

            if (oldIngredientMeasurementHasBeenRemoved)
            {
                _context.Entry(oldIngredientMeasurement).State = EntityState.Deleted;
            }
        }
    }
}
