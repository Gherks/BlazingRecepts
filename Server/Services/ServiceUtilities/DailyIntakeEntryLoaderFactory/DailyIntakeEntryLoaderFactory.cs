using BlazingRecept.Contract;
using BlazingRecept.Logging;
using BlazingRecept.Server.Services.ServiceUtilities.DailyIntakeEntryLoaders;

namespace BlazingRecept.Server.Services.ServiceUtilities.DailyIntakeEntryLoaderFactory;

public sealed class DailyIntakeEntryLoaderFactory : IDailyIntakeEntryLoaderFactory
{
    IServiceProvider _serviceProvider;

    public DailyIntakeEntryLoaderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IDailyIntakeEntryLoader Create(string name)
    {
        IDailyIntakeEntryLoader? dailyIntakeEntryLoader;

        switch (name.ToLowerInvariant())
        {
            case "recipe":
                dailyIntakeEntryLoader = _serviceProvider.GetService<DailyIntakeEntryFromRecipeLoader>();
                break;
            case "ingredient":
                dailyIntakeEntryLoader = _serviceProvider.GetService<DailyIntakeEntryFromIngredientLoader>();
                break;
            default:
                string errorMessage = $"Cannot create daily intake entry loader with given name: {name}";
                Log.Error(errorMessage);
                throw new KeyNotFoundException(errorMessage);
        }

        Contracts.LogAndThrowWhenNull(dailyIntakeEntryLoader, $"Failed to fetch daily intake entry loader from service provider with given name: {name}");

        return dailyIntakeEntryLoader;
    }
}
