using BlazingRecept.Server.Context;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BlazingRecept.Server.Repositories;

public class RecipeRepository : RepositoryBase<Recipe>, IRecipeRepository
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "RecipeRepository";

    public RecipeRepository(BlazingReceptContext context) : base(context)
    {
    }

    public async Task<bool> AnyAsync(string name)
    {
        try
        {
            return await _context.Recipe.AnyAsync(ingredient => ingredient.Name.ToLower() == name.ToLower());
        }
        catch (Exception exception)
        {
            const string errorMessage = "Repository failed check existence of recipe with name: {@Name}";
            Log.ForContext(_logProperty, _logDomainName).Error(exception, errorMessage, name);

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
        catch (Exception exception)
        {
            const string errorMessage = "Repository failed to fetch recipe with id: {@Id}";
            Log.ForContext(_logProperty, _logDomainName).Error(exception, errorMessage, id);

            return null;
        }
    }

    public async Task<Recipe?> GetByNameAsync(string name)
    {
        try
        {
            return await _context.Set<Recipe>().FirstAsync(recipe => recipe.Name.ToLower() == name.ToLower());
        }
        catch (Exception exception)
        {
            const string errorMessage = "Repository failed to fetch recipe with name: {@Name}";
            Log.ForContext(_logProperty, _logDomainName).Error(exception, errorMessage, name);

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
        catch (Exception exception)
        {
            const string errorMessage = "Repository failed to fetch all recipes.";
            Log.ForContext(_logProperty, _logDomainName).Error(exception, errorMessage);

            return null;
        }
    }

    public override async Task<Recipe> AddAsync(Recipe recipe)
    {
        try
        {
            foreach (IngredientMeasurement ingredientMeasurement in recipe.IngredientMeasurements)
            {
                ingredientMeasurement.Ingredient = await _context.Ingredient
                    .Where(ingredient => ingredient.Id == ingredientMeasurement.Ingredient.Id)
                    .Include(ingredient => ingredient.Category)
                    .FirstAsync();
            }

            _context.Add(recipe);

            await _context.SaveChangesAsync();
            await _context.Entry(recipe).ReloadAsync();
        }
        catch (Exception exception)
        {
            const string errorMessage = "Repository failed to add recipe: {@Recipe}";
            Log.ForContext(_logProperty, _logDomainName).Error(exception, errorMessage, recipe);
        }

        return recipe;
    }

    public override async Task<Recipe> UpdateAsync(Recipe updatedRecipe)
    {
        Recipe currentRecipe = await GetByIdAsync(updatedRecipe.Id) ?? throw new InvalidOperationException();

        try
        {
            UpdateRecipeIngredientMeasurements(currentRecipe, updatedRecipe);

            currentRecipe.Name = updatedRecipe.Name;
            currentRecipe.Instructions = updatedRecipe.Instructions;
            currentRecipe.PortionAmount = updatedRecipe.PortionAmount;
            currentRecipe.CategoryId = updatedRecipe.CategoryId;

            _context.Entry(currentRecipe).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            await _context.Entry(currentRecipe).ReloadAsync();

        }
        catch (Exception exception)
        {
            const string errorMessage = "Repository failed to update recipe: {@Recipe}";
            Log.ForContext(_logProperty, _logDomainName).Error(exception, errorMessage, currentRecipe);
        }

        return currentRecipe;
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
