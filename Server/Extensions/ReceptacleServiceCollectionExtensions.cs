using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Receptacle.Server.Context;
using Receptacle.Server.Repositories;
using Receptacle.Server.Repositories.Interfaces;
using Receptacle.Server.Services;
using Receptacle.Server.Services.Interfaces;

namespace Receptacle.Server.Extensions;

public static class ReceptacleServiceCollectionExtensions
{
    public static IServiceCollection AddReceptacleServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Context
        services.AddDbContext<ReceptacleContext>(options => options.UseSqlServer(configuration.GetConnectionString("ReceptacleLocal")));

        // Repositories
        services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
        services.AddScoped<IIngredientsRepository, IngredientsRepository>();
        services.AddScoped<IIngredientCategoryRepository, IngredientCategoryRepository>();

        // Services
        services.AddScoped<IIngredientsService, IngredientsService>();
        services.AddScoped<IIngredientCategoryService, IngredientCategoryService>();

        return services;
    }
}
