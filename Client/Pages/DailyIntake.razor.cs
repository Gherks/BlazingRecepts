using BlazingRecept.Client.Components.Common;
using BlazingRecept.Client.Components.PageComponents.DailyIntakePage;
using BlazingRecept.Client.Pages.Base;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Contract;
using BlazingRecept.Shared.Dto;
using BlazingRecept.Shared.Extensions;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Pages;

public partial class DailyIntake : PageBase
{
    private AddDailyIntakeEntryModal? _addDailyIntakeEntryModal;
    private RemovalConfirmationModal<DailyIntakeEntryDto>? _removalConfirmationModal;
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

    [Inject]
    protected internal IHxMessengerService? MessengerService { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        Contracts.LogAndThrowWhenNull(IngredientService, "Cannot fetch ingredients because ingredient service is not set.");
        Contracts.LogAndThrowWhenNull(RecipeService, "Cannot fetch recipes because recipe service is not set.");

        IsLoading = true;

        await base.OnInitializedAsync();

        await LoadDailyIntakeEntriesToCollections();

        Ingredients = await IngredientService.GetAllAsync();

        Contracts.LogAndThrowWhenNull(Ingredients, "Cannot properly present daily intake page because it could not fetch list of all ingredients.");

        Recipes = await RecipeService.GetAllAsync();

        Contracts.LogAndThrowWhenNull(Recipes, "Cannot properly present daily intake page because it could not fetch list of all recipes.");

        IsLoading = false;
    }

    public void Refresh()
    {
        StateHasChanged();
    }

    private async Task<bool> HandleDailyIntakeEntryAddAsync(DailyIntakeTable dailyIntakeTable, Guid collectionId)
    {
        Contracts.LogAndThrowWhenNull(_addDailyIntakeEntryModal, "Cannot open add daily intake entry modal because modal has not been set.");

        await _addDailyIntakeEntryModal.Open(dailyIntakeTable, collectionId);
        return await Task.FromResult(true);
    }

    private async Task<bool> HandleDailyIntakeEntryMoveUpInOrderAsync(DailyIntakeTable dailyIntakeTable, DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        Contracts.LogAndThrowWhenNull(DailyIntakeEntryService, "Daily intake entry service has not been set before moving daily intake entry up in order.");

        List<DailyIntakeEntryDto> dailyIntakeEntries = DailyIntakeEntryDtoCollections[dailyIntakeEntryDto.CollectionId];

        int movedIndex = dailyIntakeEntries.IndexOf(dailyIntakeEntryDto);

        if (movedIndex != 0)
        {
            dailyIntakeEntries.Swap(movedIndex, movedIndex - 1);
        }

        UpdateSortOrderInDailyIntakeEntryCollection(dailyIntakeEntries);
        bool reorderSuccessful = await DailyIntakeEntryService.SaveAsync(dailyIntakeEntries);

        if (reorderSuccessful)
        {
            dailyIntakeTable.ConstructCheckableDailyIntakeEntryList(dailyIntakeEntries);
            return true;
        }

        return false;
    }

    private async Task<bool> HandleDailyIntakeEntryMoveDownInOrderAsync(DailyIntakeTable dailyIntakeTable, DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        Contracts.LogAndThrowWhenNull(DailyIntakeEntryService, "Daily intake entry service has not been set before moving daily intake entry down in order.");

        List<DailyIntakeEntryDto> dailyIntakeEntries = DailyIntakeEntryDtoCollections[dailyIntakeEntryDto.CollectionId];

        int movedIndex = dailyIntakeEntries.IndexOf(dailyIntakeEntryDto);

        if (movedIndex < dailyIntakeEntries.Count - 1)
        {
            dailyIntakeEntries.Swap(movedIndex, movedIndex + 1);
        }

        UpdateSortOrderInDailyIntakeEntryCollection(dailyIntakeEntries);
        bool reorderSuccessful = await DailyIntakeEntryService.SaveAsync(dailyIntakeEntries);

        if (reorderSuccessful)
        {
            dailyIntakeTable.ConstructCheckableDailyIntakeEntryList(dailyIntakeEntries);
            return true;
        }

        return false;
    }

    private async Task<bool> HandleDailyIntakeEntryEditSubmitAsync(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        Contracts.LogAndThrowWhenNull(DailyIntakeEntryService, "Cannot save edited daily intake entry because the daily intake entry service has not been set.");

        DailyIntakeEntryDto? savedDailyIntakeEntryDto = await DailyIntakeEntryService.SaveAsync(dailyIntakeEntryDto);

        Contracts.LogAndThrowWhenNull(savedDailyIntakeEntryDto, "Something went wrong when saving an edited daily intake entry.");

        UpsertDailyIntakeEntryIntoCollection(savedDailyIntakeEntryDto);
        StateHasChanged();

        return true;
    }

    private async Task<bool> HandleDailyIntakeEntryRemoveAsync(DailyIntakeTable dailyIntakeTable, DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        Contracts.LogAndThrowWhenNull(_removalConfirmationModal, "Confirmation modal cannot be opened because it has not been set.");

        _dailyIntakeTable = dailyIntakeTable;
        await _removalConfirmationModal.Open(dailyIntakeEntryDto, "Ta bort post för dagligt intag", dailyIntakeEntryDto.ProductName);
        return await Task.FromResult(true);
    }

    private void HandleAddNewCollectionClick()
    {
        DailyIntakeEntryDtoCollections[Guid.NewGuid()] = new();
        StateHasChanged();
    }

    private static void UpdateSortOrderInDailyIntakeEntryCollection(List<DailyIntakeEntryDto> dailyIntakeEntries)
    {
        for (int index = 0; index < dailyIntakeEntries.Count; ++index)
        {
            dailyIntakeEntries[index].SortOrder = index;
        }
    }

    private async Task HandleDailyIntakeEntryRemovalConfirmed(DailyIntakeEntryDto removedDailyIntakeEntryDto)
    {
        Contracts.LogAndThrowWhenNull(DailyIntakeEntryService, "Daily intake entry service is not available during daily intake entry removal.");
        Contracts.LogAndThrowWhenNull(MessengerService, "Messenger service is not available during daily intake entry removal.");
        Contracts.LogAndThrowWhenNull(_dailyIntakeTable, "Daily intake entry table is not available during daily intake entry removal.");

        Guid removedId = removedDailyIntakeEntryDto.Id;
        Guid removedCollectionId = removedDailyIntakeEntryDto.CollectionId;

        bool removalFromDatabaseSuccessful = await DailyIntakeEntryService.DeleteAsync(removedId);
        bool removalFromCollectionSuccessful = false;

        foreach (List<DailyIntakeEntryDto> dailyIntakeEntryDtos in DailyIntakeEntryDtoCollections.Values)
        {
            DailyIntakeEntryDto? soughtDailyIntakeEntryDto = dailyIntakeEntryDtos.Find(dailyIntakeEntryDto => dailyIntakeEntryDto.Id == removedId);

            if (soughtDailyIntakeEntryDto != null)
            {
                dailyIntakeEntryDtos.Remove(soughtDailyIntakeEntryDto);

                if (dailyIntakeEntryDtos.Count == 0)
                {
                    DailyIntakeEntryDtoCollections.Remove(removedCollectionId);
                }
                else
                {
                    _dailyIntakeTable.ConstructCheckableDailyIntakeEntryList(dailyIntakeEntryDtos);
                    _dailyIntakeTable = null;
                }

                removalFromCollectionSuccessful = true;
                break;
            }
        }

        if (removalFromDatabaseSuccessful && removalFromCollectionSuccessful)
        {
            MessengerService.AddInformation("Dagligt intag", "Post för dagligt intag borttagen.");
            StateHasChanged();
        }
    }

    private async Task LoadDailyIntakeEntriesToCollections()
    {
        Contracts.LogAndThrowWhenNull(DailyIntakeEntryService, "Cannot fetch daily intake entry because daily intake service is not set.");

        IReadOnlyList<DailyIntakeEntryDto>? dailyIntakeEntryDtos = await DailyIntakeEntryService.GetAllAsync();

        Contracts.LogAndThrowWhenNull(dailyIntakeEntryDtos, "Cannot load daily intake entry collection because fetched list of entries was null.");

        foreach (DailyIntakeEntryDto dailyIntakeEntryDto in dailyIntakeEntryDtos)
        {
            UpsertDailyIntakeEntryIntoCollection(dailyIntakeEntryDto);
        }

        SortDailyIntakeEntryCollections();
    }

    internal List<DailyIntakeEntryDto> UpsertDailyIntakeEntryIntoCollection(DailyIntakeEntryDto upsertedDailyIntakeEntryDto)
    {
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

        return currentCollection;
    }

    private void HandleUnmappedCollection(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        bool collectionWithKeyIdExists = DailyIntakeEntryDtoCollections.ContainsKey(dailyIntakeEntryDto.CollectionId);

        if (collectionWithKeyIdExists == false)
        {
            DailyIntakeEntryDtoCollections[dailyIntakeEntryDto.CollectionId] = new();
        }
    }

    private void SortDailyIntakeEntryCollections()
    {
        foreach (List<DailyIntakeEntryDto> dailyIntakeEntryDtos in DailyIntakeEntryDtoCollections.Values)
        {
            dailyIntakeEntryDtos.Sort((first, second) => first.SortOrder > second.SortOrder ? 1 : -1);
        }
    }
}
