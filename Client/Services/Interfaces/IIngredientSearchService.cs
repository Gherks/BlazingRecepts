using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Client.Services.Interfaces;

public interface IIngredientSearchService
{
    Task<IReadOnlyList<IngredientSearchResultDto>> SearchAsync(string query);
}
