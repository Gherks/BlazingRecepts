using BlazingRecept.Server.Entities;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Server.Repositories.Interfaces;

public interface ICategoryRepository : IAsyncRepository<Category>
{
    Task<IReadOnlyList<Category>?> ListAllOfTypeAsync(CategoryType categoryType);
}
