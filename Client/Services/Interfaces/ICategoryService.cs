using BlazingRecept.Shared.Dto;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Client.Services.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>?> GetAllOfTypeAsync(CategoryType categoryType);
}
