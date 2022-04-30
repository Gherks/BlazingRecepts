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
    internal IReadOnlyList<IngredientDto>? Ingredients { get; private set; } = null;
    internal IReadOnlyList<RecipeDto>? Recipes { get; private set; } = null;

    [Inject]
    protected internal IDailyIntakeEntryService? DailyIntakeEntryService { get; private set; }

    [Inject]
    protected internal IIngredientService? IngredientService { get; private set; }

    [Inject]
    protected internal IRecipeService? RecipeService { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        await base.OnInitializedAsync();

        if (IngredientService == null)
        {
            const string errorMessage = "Cannot fetch ingredients because ingredient service is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (RecipeService == null)
        {
            const string errorMessage = "Cannot fetch recipes because recipe service is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        await LoadDailyIntakeEntriesToCollections();

        Ingredients = await IngredientService.GetAllAsync();

        if (Ingredients == null)
        {
            const string errorMessage = "Cannot properly present daily intake page because it could not fetch list of all ingredients.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        Recipes = await RecipeService.GetAllAsync();

        if (Recipes == null)
        {
            const string errorMessage = "Cannot properly present daily intake page because it could not fetch list of all recipes.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

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
            const string errorMessage = "Cannot upsert daily intake collection because daily intake table is not set.";
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
            const string errorMessage = "Cannot fetch daily intake entry because daily intake service is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        IReadOnlyList<DailyIntakeEntryDto>? dailyIntakeEntryDtos = await DailyIntakeEntryService.GetAllAsync();

        if (dailyIntakeEntryDtos == null)
        {
            const string errorMessage = "Cannot load daily intake entry collection because fetched list of entries was null.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        foreach (DailyIntakeEntryDto dailyIntakeEntryDto in dailyIntakeEntryDtos)
        {
            InsertDailyIntakeEntryIntoCollection(dailyIntakeEntryDto);
        }

        SortDailyIntakeEntryCollections();
    }

    private void SortDailyIntakeEntryCollections()
    {
        foreach (List<DailyIntakeEntryDto> dailyIntakeEntryDtos in DailyIntakeEntryDtoCollections.Values)
        {
            dailyIntakeEntryDtos.Sort((first, second) => first.SortOrder > second.SortOrder ? 1 : -1);
        }
    }
}
