using BlazingRecept.Contract;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Mappers;
using BlazingRecept.Server.Repositories.Interfaces;
using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;

    public RecipeService(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    public async Task<bool> AnyAsync(Guid id)
    {
        return await _recipeRepository.AnyAsync(id);
    }

    public async Task<bool> AnyAsync(string name)
    {
        return await _recipeRepository.AnyAsync(name);
    }

    public async Task<RecipeDto?> GetByIdAsync(Guid id)
    {
        Recipe? recipe = await _recipeRepository.GetByIdAsync(id);

        if (recipe != null)
        {
            return EntityMapper.ToDto(recipe);
        }

        return null;
    }

    public async Task<RecipeDto?> GetByNameAsync(string name)
    {
        Recipe? recipe = await _recipeRepository.GetByNameAsync(name);

        if (recipe != null)
        {
            return EntityMapper.ToDto(recipe);
        }

        return null;
    }

    public async Task<IReadOnlyList<RecipeDto>?> GetAllAsync()
    {
        IReadOnlyList<Recipe>? recipes = await _recipeRepository.ListAllAsync();

        Contracts.LogAndThrowWhenNull(recipes, "Failed to fetch all recipes from repository.");

        return recipes.Select(recipe => EntityMapper.ToDto(recipe)).ToList();
    }

    public async Task<RecipeDto> SaveAsync(RecipeDto recipeDto)
    {
        Recipe? recipe = EntityMapper.ToEntity(recipeDto);

        if (recipe.Id == Guid.Empty)
        {
            recipe = await _recipeRepository.AddAsync(recipe);
        }
        else
        {
            recipe = await _recipeRepository.UpdateAsync(recipe);
        }

        Contracts.LogAndThrowWhenNull(recipe, "Failed to save recipe, repository returned null.");

        return EntityMapper.ToDto(recipe);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        Recipe? recipe = await _recipeRepository.GetByIdAsync(id);

        if (recipe != null)
        {
            return await _recipeRepository.DeleteAsync(recipe);
        }

        return false;
    }
}
