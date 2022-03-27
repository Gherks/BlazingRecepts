using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Client.Services.Interfaces;

public interface IRecipeService
{
    Task<bool> AnyAsync(string name);
    Task<RecipeDto?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<RecipeDto>?> GetAllAsync();
    Task<RecipeDto?> SaveAsync(RecipeDto recipeDto);
    Task<bool> DeleteAsync(Guid id);
}
