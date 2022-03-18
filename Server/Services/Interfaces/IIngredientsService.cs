using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.Services.Interfaces
{
    public interface IIngredientsService
    {
        Task<IngredientDto?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<IngredientDto>> GetAllAsync();
        Task<IngredientDto> SaveAsync(IngredientDto ingredientDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
