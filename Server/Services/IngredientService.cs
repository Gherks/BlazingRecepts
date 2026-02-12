using BlazingRecept.Contract;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Mappers;
using BlazingRecept.Server.Repositories.Interfaces;
using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.Services;

public class IngredientService : IIngredientService
{
    private readonly IIngredientRepository _ingredientsRepository;

    public IngredientService(IIngredientRepository ingredientsRepository)
    {
        _ingredientsRepository = ingredientsRepository;
    }

    public async Task<bool> AnyAsync(Guid id)
    {
        return await _ingredientsRepository.AnyAsync(id);
    }

    public async Task<bool> AnyAsync(string name)
    {
        return await _ingredientsRepository.AnyAsync(name);
    }

    public async Task<IngredientDto?> GetByIdAsync(Guid id)
    {
        Ingredient? ingredient = await _ingredientsRepository.GetByIdAsync(id);

        if (ingredient != null)
        {
            return EntityMapper.ToDto(ingredient);
        }

        return null;
    }

    public async Task<IngredientDto?> GetByNameAsync(string name)
    {
        Ingredient? ingredient = await _ingredientsRepository.GetByNameAsync(name);

        if (ingredient != null)
        {
            return EntityMapper.ToDto(ingredient);
        }

        return null;
    }

    public async Task<IReadOnlyList<IngredientDto>?> GetAllAsync()
    {
        IReadOnlyList<Ingredient>? ingredients = await _ingredientsRepository.ListAllAsync();

        Contracts.LogAndThrowWhenNull(ingredients, "Failed to fetch all ingredients from repository.");

        return ingredients.Select(ingredient => EntityMapper.ToDto(ingredient)).ToList();
    }

    public async Task<IngredientDto> SaveAsync(IngredientDto ingredientDto)
    {
        Ingredient? ingredient = EntityMapper.ToEntity(ingredientDto);

        if (ingredient.Id == Guid.Empty)
        {
            ingredient = await _ingredientsRepository.AddAsync(ingredient);
        }
        else
        {
            ingredient = await _ingredientsRepository.UpdateAsync(ingredient);
        }

        Contracts.LogAndThrowWhenNull(ingredient, "Failed to save ingredient, repository returned null.");

        return EntityMapper.ToDto(ingredient);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        Ingredient? ingredient = await _ingredientsRepository.GetByIdAsync(id);

        if (ingredient != null)
        {
            return await _ingredientsRepository.DeleteAsync(ingredient);
        }

        return false;
    }
}
