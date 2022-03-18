using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.Services.Interfaces
{
    public interface IIngredientCategoryService
    {
        Task<IReadOnlyList<IngredientCategoryDto>> GetAllAsync();
    }
}
