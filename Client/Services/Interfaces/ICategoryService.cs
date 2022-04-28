using BlazingRecept.Shared.Dto;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Client.Services.Interfaces;

public interface ICategoryService
{
    Task<CategoryDto?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<CategoryDto>?> GetAllOfTypeAsync(CategoryType categoryType);
}
