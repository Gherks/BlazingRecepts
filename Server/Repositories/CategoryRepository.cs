using BlazingRecept.Server.Context;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Server.Repositories;

public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
{
    public CategoryRepository(BlazingReceptContext context) : base(context)
    {
    }
    public async Task<IReadOnlyList<Category>?> ListAllOfTypeAsync(CategoryType categoryType)
    {
        try
        {
            return await _context.Category
                .Where(category => category.CategoryType == categoryType)
                .ToListAsync();
        }
        catch (Exception)
        {
            return null;
        }
    }
}
