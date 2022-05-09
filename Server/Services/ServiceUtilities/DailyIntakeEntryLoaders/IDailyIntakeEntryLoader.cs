using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.Services.ServiceUtilities.DailyIntakeEntryLoaders;

public interface IDailyIntakeEntryLoader
{
    Task<Guid?> GetProductIdFromProductName(string productName);
    Task<DailyIntakeEntryDto?> LoadProductDataForDailyIntakeEntryDto(DailyIntakeEntryDto dailyIntakeEntryDto);
}
