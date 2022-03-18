using BlazingRecept.Server.Context;
using BlazingRecept.Server.Repositories;
using BlazingRecept.Server.Repositories.Interfaces;
using BlazingRecept.Server.Services;
using BlazingRecept.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazingRecept.Server.Extensions;

public static class BlazingReceptServiceCollectionExtensions
{
    public static WebApplicationBuilder AddBlazingReceptDbContext(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        builder.Services.AddDbContext<BlazingReceptContext>(options => options.UseSqlServer(configuration["BlazingReceptConnectionString"]));

        return builder;
    }

    public static WebApplicationBuilder AddBlazingReceptServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IIngredientsService, IngredientsService>();
        builder.Services.AddScoped<IIngredientCategoryService, IngredientCategoryService>();

        return builder;
    }

    public static WebApplicationBuilder AddBlazingReceptRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
        builder.Services.AddScoped<IIngredientsRepository, IngredientsRepository>();
        builder.Services.AddScoped<IIngredientCategoryRepository, IngredientCategoryRepository>();

        return builder;
    }
}
