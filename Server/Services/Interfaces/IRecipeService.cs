using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.Services.Interfaces;

public interface IRecipeService
{
    Task<bool> AnyAsync(Guid id);
    Task<bool> AnyAsync(string name);
    Task<RecipeDto?> GetByIdAsync(Guid id);
    Task<RecipeDto?> GetByNameAsync(string name);
    Task<IReadOnlyList<RecipeDto>?> GetAllAsync();
    Task<RecipeDto> SaveAsync(RecipeDto recipeDto);
    Task<bool> DeleteAsync(Guid id);
}
