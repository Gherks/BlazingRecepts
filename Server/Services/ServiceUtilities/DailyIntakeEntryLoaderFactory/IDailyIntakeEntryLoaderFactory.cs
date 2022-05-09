using BlazingRecept.Server.Services.ServiceUtilities.DailyIntakeEntryLoaders;

namespace BlazingRecept.Server.Services.ServiceUtilities.DailyIntakeEntryLoaderFactory;

public interface IDailyIntakeEntryLoaderFactory
{
    IDailyIntakeEntryLoader Create(string name);
}
