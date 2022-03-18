using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.Services.Interfaces
{
    public interface IRecipesService
    {
        Task<RecipeDto?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<RecipeDto>> GetAllAsync();
        Task<RecipeDto> SaveAsync(RecipeDto recipeDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
