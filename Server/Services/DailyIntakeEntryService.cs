using BlazingRecept.Contract;
using BlazingRecept.Logging;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Mappers;
using BlazingRecept.Server.Repositories.Interfaces;
using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Server.Services.ServiceUtilities.DailyIntakeEntryLoaderFactory;
using BlazingRecept.Server.Services.ServiceUtilities.DailyIntakeEntryLoaders;
using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.Services;

public class DailyIntakeEntryService : IDailyIntakeEntryService
{
    private readonly IDailyIntakeEntryRepository _dailyIntakeEntryRepository;
    private readonly IReadOnlyList<IDailyIntakeEntryLoader> dailyIntakeEntryLoaders;

    public DailyIntakeEntryService(
        IDailyIntakeEntryRepository dailyIntakeEntryRepository,
        IDailyIntakeEntryLoaderFactory dailyIntakeEntryLoaderFactory)
    {
        _dailyIntakeEntryRepository = dailyIntakeEntryRepository;

        dailyIntakeEntryLoaders = new List<IDailyIntakeEntryLoader>()
        {
            dailyIntakeEntryLoaderFactory.Create("Recipe"),
            dailyIntakeEntryLoaderFactory.Create("Ingredient")
        };
    }

    public async Task<bool> AnyAsync(Guid id)
    {
        return await _dailyIntakeEntryRepository.AnyAsync(id);
    }

    public async Task<DailyIntakeEntryDto?> GetByIdAsync(Guid id)
    {
        DailyIntakeEntry? dailyIntakeEntry = await _dailyIntakeEntryRepository.GetByIdAsync(id);

        if (dailyIntakeEntry != null)
        {
            return await LoadDailyIntakeEntryDtoFromDailyIntakeEntry(dailyIntakeEntry);
        }

        return null;
    }

    public async Task<IReadOnlyList<DailyIntakeEntryDto>?> GetAllAsync()
    {
        IReadOnlyList<DailyIntakeEntry>? dailyIntakeEntries = await _dailyIntakeEntryRepository.ListAllAsync();

        Contracts.LogAndThrowWhenNull(dailyIntakeEntries, "Failed to fetch all daily intake entries from repository.");

        List<DailyIntakeEntryDto> dailyIntakeEntryDtos = new();

        foreach (DailyIntakeEntry dailyIntakeEntry in dailyIntakeEntries)
        {
            DailyIntakeEntryDto? dailyIntakeEntryDto = await LoadDailyIntakeEntryDtoFromDailyIntakeEntry(dailyIntakeEntry);

            Contracts.LogAndThrowWhenNull(dailyIntakeEntryDto, "Failed while loading one of many daily intake entries into a daily intake entry dto.");

            dailyIntakeEntryDtos.Add(dailyIntakeEntryDto);
        }

        return dailyIntakeEntryDtos;
    }

    public async Task<DailyIntakeEntryDto> SaveAsync(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        DailyIntakeEntry? dailyIntakeEntry = EntityMapper.ToEntity(dailyIntakeEntryDto);

        dailyIntakeEntry.ProductId = await GetProductIdFromProductName(dailyIntakeEntryDto.ProductName);

        if (dailyIntakeEntry.Id == Guid.Empty)
        {
            dailyIntakeEntry = await _dailyIntakeEntryRepository.AddAsync(dailyIntakeEntry);
        }
        else
        {
            dailyIntakeEntry = await _dailyIntakeEntryRepository.UpdateAsync(dailyIntakeEntry);
        }

        Contracts.LogAndThrowWhenNull(dailyIntakeEntry, $"Failed to add or update daily intake entry({dailyIntakeEntryDto}), result was null.");

        DailyIntakeEntryDto? reloadedDailyIntakeEntryDto = await LoadDailyIntakeEntryDtoFromDailyIntakeEntry(dailyIntakeEntry);

        Contracts.LogAndThrowWhenNull(reloadedDailyIntakeEntryDto, $"Failed to reload added or updated daily intake entry({dailyIntakeEntryDto}), result was null.");

        return reloadedDailyIntakeEntryDto;
    }

    public async Task<bool> SaveAsync(List<DailyIntakeEntryDto> dailyIntakeEntryDtos)
    {
        foreach (DailyIntakeEntryDto dailyIntakeEntryDto in dailyIntakeEntryDtos)
        {
            DailyIntakeEntry dailyIntakeEntry = EntityMapper.ToEntity(dailyIntakeEntryDto);

            dailyIntakeEntry.ProductId = await GetProductIdFromProductName(dailyIntakeEntryDto.ProductName);

            try
            {
                if (dailyIntakeEntry.Id == Guid.Empty)
                {
                    await _dailyIntakeEntryRepository.AddAsync(dailyIntakeEntry);
                }
                else
                {
                    await _dailyIntakeEntryRepository.UpdateAsync(dailyIntakeEntry);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, $"Something went wrong while saving and/or updating multiple daily intake entries: {dailyIntakeEntryDtos}");
            }
        }

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        DailyIntakeEntry? dailyIntakeEntry = await _dailyIntakeEntryRepository.GetByIdAsync(id);

        if (dailyIntakeEntry != null)
        {
            return await _dailyIntakeEntryRepository.DeleteAsync(dailyIntakeEntry);
        }

        return false;
    }

    private async Task<Guid> GetProductIdFromProductName(string productName)
    {
        foreach (IDailyIntakeEntryLoader dailyIntakeEntryLoader in dailyIntakeEntryLoaders)
        {
            Guid? productId = await dailyIntakeEntryLoader.GetProductIdFromProductName(productName);

            if (productId != null)
            {
                return productId.Value;
            }
        }

        const string errorMessage = "Could not find product id from given product name because product with given name was not found in any daily intake entry loader.";
        Log.Error(errorMessage);
        throw new InvalidOperationException(errorMessage);
    }

    private async Task<DailyIntakeEntryDto?> LoadDailyIntakeEntryDtoFromDailyIntakeEntry(DailyIntakeEntry dailyIntakeEntry)
    {
        DailyIntakeEntryDto dailyIntakeEntryDto = EntityMapper.ToDto(dailyIntakeEntry);

        foreach (IDailyIntakeEntryLoader dailyIntakeEntryLoader in dailyIntakeEntryLoaders)
        {
            DailyIntakeEntryDto? loadedDailyIntakeEntryDto = await dailyIntakeEntryLoader.LoadProductDataForDailyIntakeEntryDto(dailyIntakeEntryDto);

            if (loadedDailyIntakeEntryDto != null)
            {
                return loadedDailyIntakeEntryDto;
            }
        }

        const string errorMessage = "Could not load daily intake entry dto from any daily intake entry loader.";
        Log.Error(errorMessage);
        throw new InvalidOperationException(errorMessage);
    }
}
