using BlazingRecept.Contract;
using BlazingRecept.Logging;
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
            // Use EF.Functions.Like for case-insensitive comparison without string allocation
            return await _context.Recipe.AnyAsync(recipe => EF.Functions.Like(recipe.Name, name));
        }
        catch (Exception exception)
        {
            Log.Error(exception, $"Repository failed check existence of recipe with name: {name}");
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
            Log.Error(exception, $"Repository failed to fetch recipe with id: {id}");
            return null;
        }
    }

    public async Task<Recipe?> GetByNameAsync(string name)
    {
        try
        {
            return await _context.Set<Recipe>()
                .Include(recipe => recipe.Category)
                .Include(recipe => recipe.IngredientMeasurements)
                    .ThenInclude(ingredientMeasurement => ingredientMeasurement.Ingredient)
                        .ThenInclude(ingredient => ingredient.Category)
                .FirstAsync(recipe => EF.Functions.Like(recipe.Name, name));
        }
        catch (Exception exception)
        {
            Log.Error(exception, $"Repository failed to fetch recipe with name: {name}");
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
            Log.Error(exception, "Repository failed to fetch all recipes.");
            return null;
        }
    }

    public override async Task<Recipe?> AddAsync(Recipe recipe)
    {
        try
        {
            // Batch load all ingredients to avoid N+1 query problem
            var ingredientIds = recipe.IngredientMeasurements.Select(im => im.IngredientId).ToList();
            var ingredients = await _context.Ingredient
                .Where(ingredient => ingredientIds.Contains(ingredient.Id))
                .Include(ingredient => ingredient.Category)
                .ToDictionaryAsync(ingredient => ingredient.Id);

            foreach (IngredientMeasurement ingredientMeasurement in recipe.IngredientMeasurements)
            {
                ingredientMeasurement.Ingredient = ingredients[ingredientMeasurement.IngredientId];
            }

            recipe.Category = await _context.Category
                .Where(category => category.Id == recipe.CategoryId)
                .FirstAsync();

            _context.Add(recipe);

            await _context.SaveChangesAsync();
            await _context.Entry(recipe).ReloadAsync();
        }
        catch (Exception exception)
        {
            Log.Error(exception, $"Repository failed to add recipe: {recipe}");
            return null;
        }

        return recipe;
    }

    public override async Task<Recipe> UpdateAsync(Recipe updatedRecipe)
    {
        Recipe? currentRecipe = await GetByIdAsync(updatedRecipe.Id);

        Contracts.LogAndThrowWhenNull(currentRecipe, $"Failed to fetch current state of recipe before updating it: {updatedRecipe}");

        try
        {
            await UpdateRecipeIngredientMeasurements(currentRecipe, updatedRecipe);

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
            Log.Error(exception, $"Repository failed to update recipe: {currentRecipe}");
        }

        return currentRecipe;
    }

    private async Task UpdateRecipeIngredientMeasurements(Recipe currentRecipe, Recipe updatedRecipe)
    {
        // Build dictionary for O(1) lookup instead of O(n) for each iteration
        var updatedMeasurementsById = updatedRecipe.IngredientMeasurements
            .Where(im => im.Id != Guid.Empty)
            .ToDictionary(im => im.Id);

        // Process updates and deletions using reverse iteration to safely remove items
        for (int index = currentRecipe.IngredientMeasurements.Count - 1; index >= 0; --index)
        {
            IngredientMeasurement ingredientMeasurementInCurrentRecipe = currentRecipe.IngredientMeasurements[index];

            if (updatedMeasurementsById.TryGetValue(ingredientMeasurementInCurrentRecipe.Id, out var ingredientMeasurementInUpdatedRecipe))
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
                currentRecipe.IngredientMeasurements.RemoveAt(index);
                _context.Entry(ingredientMeasurementInCurrentRecipe).State = EntityState.Deleted;
            }
        }

        // Process new ingredient measurements
        var newIngredientMeasurements = updatedRecipe.IngredientMeasurements
            .Where(im => im.Id == Guid.Empty)
            .ToList();

        if (newIngredientMeasurements.Any())
        {
            // Batch load all new ingredients to avoid N+1 query problem
            var newIngredientIds = newIngredientMeasurements.Select(im => im.IngredientId).ToList();
            var ingredients = await _context.Ingredient
                .Where(ingredient => newIngredientIds.Contains(ingredient.Id))
                .Include(ingredient => ingredient.Category)
                .ToDictionaryAsync(ingredient => ingredient.Id);

            foreach (IngredientMeasurement ingredientMeasurementInUpdatedRecipe in newIngredientMeasurements)
            {
                ingredientMeasurementInUpdatedRecipe.Ingredient = ingredients[ingredientMeasurementInUpdatedRecipe.IngredientId];
                currentRecipe.IngredientMeasurements.Add(ingredientMeasurementInUpdatedRecipe);
            }
        }
    }
}
