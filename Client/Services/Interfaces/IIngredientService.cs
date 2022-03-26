using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Client.Services.Interfaces
{
    public interface IIngredientService
    {
        Task<bool> AnyAsync(string name);
        Task<IngredientDto?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<IngredientCollectionTypeDto>?> GetAllSortedAsync();
        Task<IngredientDto?> SaveAsync(IngredientDto ingredientDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
