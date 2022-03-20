using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.Services.Interfaces
{
    public interface IIngredientService
    {
        Task<bool> AnyAsync(Guid id);
        Task<bool> AnyAsync(string name);
        Task<IngredientDto?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<IngredientDto>> GetAllAsync();
        Task<List<IngredientCollectionTypeDto>> GetAllSortedAsync();
        Task<IngredientDto> SaveAsync(IngredientDto ingredientDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
