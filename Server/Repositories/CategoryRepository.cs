using BlazingRecept.Server.Context;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Server.Repositories;

public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
{
    private static readonly string _logProperty = "Domain";

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
        catch (Exception exception)
        {
            string messageTemplate = "Repository failed to delete recipe with id: {@CategoryType}";
            Log.ForContext(_logProperty, GetType().Name).Error(exception, messageTemplate, categoryType);

            return null;
        }
    }
}
