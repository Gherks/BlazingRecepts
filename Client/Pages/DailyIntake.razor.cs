using BlazingRecept.Client.Components.PageComponents.DailyIntakePage;
using BlazingRecept.Client.Pages.Base;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace BlazingRecept.Client.Pages;

public partial class DailyIntake : PageBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "DailyIntake";

    private DailyIntakeTable? _dailyIntakeTable;

    internal Dictionary<Guid, List<DailyIntakeEntryDto>> DailyIntakeEntryDtoCollections { get; private set; } = new();
    public IReadOnlyList<IngredientDto> Ingredients { get; private set; } = new List<IngredientDto>();
    public IReadOnlyList<RecipeDto> Recipes { get; private set; } = new List<RecipeDto>();

    [Inject]
    public IDailyIntakeEntryService? DailyIntakeEntryService { get; private set; }

    [Inject]
    public IIngredientService? IngredientService { get; private set; }

    [Inject]
    public IRecipeService? RecipeService { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        await base.OnInitializedAsync();

        if (IngredientService == null)
        {
            string errorMessage = "Cannot fetch ingredients because ingredient service is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (RecipeService == null)
        {
            string errorMessage = "Cannot fetch recipes because recipe service is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        await LoadDailyIntakeEntriesToCollections();

        Ingredients = await IngredientService.GetAllAsync() ?? new List<IngredientDto>();
        Recipes = await RecipeService.GetAllAsync() ?? new List<RecipeDto>();

        IsLoading = false;
    }

    internal void InsertDailyIntakeEntryIntoCollection(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        HandleUnmappedCollection(dailyIntakeEntryDto);

        DailyIntakeEntryDtoCollections[dailyIntakeEntryDto.CollectionId].Add(dailyIntakeEntryDto);
    }

    internal void UpsertDailyIntakeEntryIntoCollection(DailyIntakeEntryDto upsertedDailyIntakeEntryDto)
    {
        if (_dailyIntakeTable == null)
        {
            string errorMessage = "Cannot upsert daily intake collection because daily intake table is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        HandleUnmappedCollection(upsertedDailyIntakeEntryDto);

        List<DailyIntakeEntryDto> currentCollection = DailyIntakeEntryDtoCollections[upsertedDailyIntakeEntryDto.CollectionId];

        int index = currentCollection.FindIndex(dailyIntakeEntryDto => dailyIntakeEntryDto.Id == upsertedDailyIntakeEntryDto.Id);

        if (index >= 0)
        {
            currentCollection[index] = upsertedDailyIntakeEntryDto;
        }
        else
        {
            currentCollection.Add(upsertedDailyIntakeEntryDto);
        }

        _dailyIntakeTable.Refresh();
    }

    private void HandleUnmappedCollection(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        bool collectionWithKeyIdExists = DailyIntakeEntryDtoCollections.ContainsKey(dailyIntakeEntryDto.CollectionId);

        if (collectionWithKeyIdExists == false)
        {
            DailyIntakeEntryDtoCollections[dailyIntakeEntryDto.CollectionId] = new();
        }
    }

    private async Task LoadDailyIntakeEntriesToCollections()
    {
        if (DailyIntakeEntryService == null)
        {
            string errorMessage = "Cannot fetch daily intake entry because daily intake service is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        IReadOnlyList<DailyIntakeEntryDto>? dailyIntakeEntryDtos = await DailyIntakeEntryService.GetAllAsync();

        if (dailyIntakeEntryDtos == null)
        {
            string errorMessage = "Cannot load daily intake entry collection because fetched list of entries was null.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        foreach (DailyIntakeEntryDto dailyIntakeEntryDto in dailyIntakeEntryDtos)
        {
            InsertDailyIntakeEntryIntoCollection(dailyIntakeEntryDto);
        }
    }
}
