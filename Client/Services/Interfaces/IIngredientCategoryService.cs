using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Client.Services.Interfaces;

public interface IIngredientCategoryService
{
    Task<IReadOnlyList<IngredientCategoryDto>?> GetAllAsync();
}
