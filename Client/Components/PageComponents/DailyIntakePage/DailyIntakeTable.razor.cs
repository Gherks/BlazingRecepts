using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Components.Utilities;
using BlazingRecept.Client.Extensions;
using BlazingRecept.Client.Pages;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace BlazingRecept.Client.Components.PageComponents.DailyIntakePage;

public partial class DailyIntakeTable : PageComponentBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "DailyIntakeTable";

    private Guid _editingDailyIntakeEntryGuid = Guid.Empty;

    private RemovalConfirmationModal<DailyIntakeEntryDto>? _removalConfirmationModal;

    [CascadingParameter]
    protected internal DailyIntake? DailyIntakePage { get; private set; }

    [Inject]
    public IDailyIntakeEntryService? DailyIntakeService { get; set; }

    [Inject]
    public IIngredientService? IngredientService { get; set; }

    [Inject]
    public IRecipeService? RecipeService { get; set; }

    [Inject]
    public IToastService? ToastService { get; set; }

    private void HandleDailyIntakeEntryEditClicked(Guid editingDailyIntakeEntryGuid)
    {
        _editingDailyIntakeEntryGuid = editingDailyIntakeEntryGuid;
        StateHasChanged();
    }

    private void HandleDailyIntakeEntryRemovalModalOpen(DailyIntakeEntryDto? dailyIntakeEntryDto)
    {
        if (_removalConfirmationModal == null)
        {
            string errorMessage = "Confirmation modal cannot be opened because it has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (dailyIntakeEntryDto == null)
        {
            string errorMessage = "Cannot start daily intake entry removal process because daily intake entry has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new ArgumentNullException(nameof(dailyIntakeEntryDto), errorMessage);
        }

        _removalConfirmationModal.Open(dailyIntakeEntryDto, "Ta bort post f�r dagligt intag", dailyIntakeEntryDto.ProductName);
    }

    private async Task HandleDailyIntakeEntryEditConfirmed(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        if (DailyIntakePage == null)
        {
            string errorMessage = "Cannot save edited daily intake entry because the daily intake page reference has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (DailyIntakeService == null)
        {
            string errorMessage = "Cannot save edited daily intake entry because the daily intake service has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (IngredientService == null)
        {
            string errorMessage = "Cannot save edited daily intake entry because the ingredient service has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (RecipeService == null)
        {
            string errorMessage = "Cannot save edited daily intake entry because the recipe service has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        DailyIntakeEntryDto? savedDailyIntakeEntryDto = await DailyIntakeService.SaveAsync(dailyIntakeEntryDto);

        if (savedDailyIntakeEntryDto == null)
        {
            string errorMessage = "Something went wrong when saving an edited daily intake entry.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        await savedDailyIntakeEntryDto.LoadFromProductServices(IngredientService, RecipeService);

        DailyIntakePage.UpsertDailyIntakeEntryIntoCollection(savedDailyIntakeEntryDto);

        _editingDailyIntakeEntryGuid = Guid.Empty;
        StateHasChanged();
    }

    private void HandleAddNewCollection()
    {
        if (DailyIntakePage == null)
        {
            string errorMessage = "Cannot add new daily intake entry collection because the daily intake page reference has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        DailyIntakePage.DailyIntakeEntryDtoCollections[Guid.NewGuid()] = new();
    }

    private void HandleDailyIntakeEntryEditCancel()
    {
        _editingDailyIntakeEntryGuid = Guid.Empty;
        StateHasChanged();
    }

    private async Task HandleDailyIntakeEntryRemovalConfirmed(DailyIntakeEntryDto removedDailyIntakeEntryDto)
    {
        if (DailyIntakePage == null)
        {
            string errorMessage = "Cannot remove daily intake entry from daily intake page because daily intake page reference is null.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (DailyIntakeService == null)
        {
            string errorMessage = "Daily intake service is not available during daily intake entry removal.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (ToastService == null)
        {
            string errorMessage = "Toast service is not available during daily intake entry removal.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        Guid removedId = removedDailyIntakeEntryDto.Id;
        Guid removedCollectionId = removedDailyIntakeEntryDto.CollectionId;

        bool removalFromDatabaseSuccessful = await DailyIntakeService.DeleteAsync(removedId);
        bool removalFromCollectionSuccessful = false;

        foreach (List<DailyIntakeEntryDto> dailyIntakeEntryDtos in DailyIntakePage.DailyIntakeEntryDtoCollections.Values)
        {
            DailyIntakeEntryDto? soughtDailyIntakeEntryDto = dailyIntakeEntryDtos.Find(dailyIntakeEntryDto => dailyIntakeEntryDto.Id == removedId);

            if (soughtDailyIntakeEntryDto != null)
            {
                dailyIntakeEntryDtos.Remove(soughtDailyIntakeEntryDto);

                if (dailyIntakeEntryDtos.Count == 0)
                {
                    DailyIntakePage.DailyIntakeEntryDtoCollections.Remove(removedCollectionId);
                }

                removalFromCollectionSuccessful = true;
                break;
            }
        }

        if (removalFromDatabaseSuccessful && removalFromCollectionSuccessful)
        {
            ToastService.ShowInfo("Post f�r dagligt intag borttagen.");
            StateHasChanged();
        }
    }

    private string GetAmount(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        if (dailyIntakeEntryDto.IsRecipe)
        {
            string portionSuffix = (dailyIntakeEntryDto.Amount > 1) ? "portioner" : "portion";

            return $"{Math.Round(dailyIntakeEntryDto.Amount, 2)} {portionSuffix}";
        }
        else
        {
            return $"{Math.Round(dailyIntakeEntryDto.Amount, 2)} gram";
        }
    }

    private double GetFat(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        return Math.Round(dailyIntakeEntryDto.Fat, 2);
    }

    private double GetCarbohydrates(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        return Math.Round(dailyIntakeEntryDto.Carbohydrates, 2);
    }

    private double GetProtein(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        return Math.Round(dailyIntakeEntryDto.Protein, 2);
    }

    private double GetCalories(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        return Math.Round(dailyIntakeEntryDto.Calories, 2);
    }

    private double GetProteinPerCalorie(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        return Math.Round(dailyIntakeEntryDto.ProteinPerCalorie, 2);
    }

    private double GetFatTotal(List<DailyIntakeEntryDto> dailyIntakeEntryDtos)
    {
        if (dailyIntakeEntryDtos == null)
        {
            string errorMessage = "Cannot present daily intake grams total because daily intake list is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        double fatTotal = dailyIntakeEntryDtos.Sum(dailyIntakeEntryDto => dailyIntakeEntryDto.Fat);

        return Math.Round(fatTotal, 2);
    }

    private double GetCarbohydrateTotal(List<DailyIntakeEntryDto> dailyIntakeEntryDtos)
    {
        if (dailyIntakeEntryDtos == null)
        {
            string errorMessage = "Cannot present daily intake carbohydrate total because daily intake list is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        double carbohydrateTotal = dailyIntakeEntryDtos.Sum(dailyIntakeEntryDto => dailyIntakeEntryDto.Carbohydrates);

        return Math.Round(carbohydrateTotal, 2);
    }

    private double GetProteinTotal(List<DailyIntakeEntryDto> dailyIntakeEntryDtos)
    {
        if (dailyIntakeEntryDtos == null)
        {
            string errorMessage = "Cannot present daily intake protein total because daily intake list is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        double proteinTotal = dailyIntakeEntryDtos.Sum(dailyIntakeEntryDto => dailyIntakeEntryDto.Protein);

        return Math.Round(proteinTotal, 2);
    }

    private double GetCalorieTotal(List<DailyIntakeEntryDto> dailyIntakeEntryDtos)
    {
        if (dailyIntakeEntryDtos == null)
        {
            string errorMessage = "Cannot present daily intake calorie total because daily intake list is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        double calorieTotal = dailyIntakeEntryDtos.Sum(dailyIntakeEntryDto => dailyIntakeEntryDto.Calories);

        return Math.Round(calorieTotal, 2);
    }

    private double GetAverageProteinPerCalorie(List<DailyIntakeEntryDto> dailyIntakeEntryDtos)
    {
        if (dailyIntakeEntryDtos == null)
        {
            string errorMessage = "Cannot present daily intake protein per gram because daily intake list is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (dailyIntakeEntryDtos.Count == 0)
        {
            return 0;
        }

        double proteinPerCalorieSum = dailyIntakeEntryDtos.Sum(dailyIntakeEntryDto => dailyIntakeEntryDto.ProteinPerCalorie);
        double proteinPerCalorieAverage = proteinPerCalorieSum / dailyIntakeEntryDtos.Count;

        return Math.Round(proteinPerCalorieAverage, 2);
    }
}