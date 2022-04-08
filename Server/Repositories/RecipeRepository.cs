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

    public override async Task<Recipe> UpdateAsync(Recipe updatedRecipe)
    {
        Recipe recipe = await GetByIdAsync(updatedRecipe.Id) ?? throw new InvalidOperationException();

        try
        {
            UpdateRecipeIngredientMeasurements(recipe, updatedRecipe);

            recipe.Name = updatedRecipe.Name;
            recipe.Instructions = updatedRecipe.Instructions;
            recipe.PortionAmount = updatedRecipe.PortionAmount;
            recipe.CategoryId = updatedRecipe.CategoryId;

            _context.Entry(recipe).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            await _context.Entry(recipe).ReloadAsync();

        }
        catch (Exception)
        {
        }

        return recipe;
    }

    private void UpdateRecipeIngredientMeasurements(Recipe currentRecipe, Recipe updatedRecipe)
    {
        for (int index = 0; index < currentRecipe.IngredientMeasurements.Count; ++index)
        {
            IngredientMeasurement ingredientMeasurementInCurrentRecipe = currentRecipe.IngredientMeasurements[index];

            IngredientMeasurement? ingredientMeasurementInUpdatedRecipe = updatedRecipe.IngredientMeasurements
                .FirstOrDefault(ingredientMeasurement => ingredientMeasurement.Id == ingredientMeasurementInCurrentRecipe.Id);

            if (ingredientMeasurementInUpdatedRecipe != null)
            {
                ingredientMeasurementInCurrentRecipe.Measurement = ingredientMeasurementInUpdatedRecipe.Measurement;
                ingredientMeasurementInCurrentRecipe.MeasurementUnit = ingredientMeasurementInUpdatedRecipe.MeasurementUnit;
                ingredientMeasurementInCurrentRecipe.Grams = ingredientMeasurementInUpdatedRecipe.Grams;
                ingredientMeasurementInCurrentRecipe.Note = ingredientMeasurementInUpdatedRecipe.Note;
                ingredientMeasurementInCurrentRecipe.SortOrder = ingredientMeasurementInUpdatedRecipe.SortOrder;
                ingredientMeasurementInCurrentRecipe.IngredientId = ingredientMeasurementInUpdatedRecipe.IngredientId;

                _context.Entry(ingredientMeasurementInCurrentRecipe).State = EntityState.Modified;
            }
            else
            {
                currentRecipe.IngredientMeasurements.Remove(ingredientMeasurementInCurrentRecipe);
                _context.Entry(ingredientMeasurementInCurrentRecipe).State = EntityState.Deleted;

                --index;
            }
        }

        foreach (IngredientMeasurement ingredientMeasurementInUpdatedRecipe in updatedRecipe.IngredientMeasurements)
        {
            if (ingredientMeasurementInUpdatedRecipe.Id == Guid.Empty)
            {
                currentRecipe.IngredientMeasurements.Add(ingredientMeasurementInUpdatedRecipe);
            }
        }
    }
}
