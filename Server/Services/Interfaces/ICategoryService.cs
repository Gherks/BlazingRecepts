using BlazingRecept.Shared.Dto;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Server.Services.Interfaces
{
    public interface ICategoryService
    {
        //Task<IReadOnlyList<CategoryDto>> GetAllAsync();

        Task<IReadOnlyList<CategoryDto>> GetAllOfTypeAsync(CategoryType categoryType);
    }
}
