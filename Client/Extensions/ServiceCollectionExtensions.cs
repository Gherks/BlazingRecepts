using BlazingRecept.Client.Services;
using BlazingRecept.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazingRecept.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static WebAssemblyHostBuilder AddHttpClients(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddHttpClient("BlazingRecept.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
        //.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

        // Supply HttpClient instances that include access tokens when making requests to the server project
        builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazingRecept.ServerAPI"));

        return builder;
    }

    public static WebAssemblyHostBuilder AddServices(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IIngredientService, IngredientService>();
        builder.Services.AddScoped<IRecipeService, RecipeService>();

        return builder;
    }
}
