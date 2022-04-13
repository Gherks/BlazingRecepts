using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Components.Utilities;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared;
using BlazingRecept.Shared.Dto;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace BlazingRecept.Client.Pages;

public partial class Recipe : PageComponentBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "RecipePage";

    private RecipeDto? _recipeDto = new RecipeDto();
    private List<CheckableIngredientMeasurement> _checkableIngredientMeasurements = new();

    private RemovalConfirmationModal<RecipeDto>? _removalConfirmationModal;

    [Parameter]
    public Guid RecipeId { get; set; }

    [Inject]
    public IRecipeService? RecipeService { get; set; }

    [Inject]
    public IToastService? ToastService { get; set; }

    [Inject]
    public NavigationManager? NavigationManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        await base.OnInitializedAsync();

        if (RecipeService == null)
        {
            string errorMessage = "Cannot fetch recipe because recipe service is null.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _recipeDto = await RecipeService.GetByIdAsync(RecipeId);

        if (_recipeDto == null)
        {
            string errorMessage = "Cannot properly present recipe because sought recipe coulnd't be fetched.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        foreach (IngredientMeasurementDto ingredientMeasurementDto in _recipeDto.IngredientMeasurementDtos)
        {
            _checkableIngredientMeasurements.Add(new()
            {
                IsChecked = false,
                IngredientMeasurementDto = ingredientMeasurementDto
            });
        }

        IsLoading = false;
    }

    private void HandleNavigationToEditRecipe(RecipeDto recipeDto)
    {
        if (NavigationManager == null)
        {
            string errorMessage = "Cannot navigate to recipe edit page because navigation manager has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        NavigationManager.NavigateTo($"recipeworkbench/{recipeDto.Id}");
    }

    private void HandleRecipeRemovalModalOpen(RecipeDto recipeDto)
    {
        if (_removalConfirmationModal == null)
        {
            string errorMessage = "Confirmation modal cannot be opened because it has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (recipeDto == null)
        {
            string errorMessage = "Cannot start recipe removal process because passed recipe was not valid.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _removalConfirmationModal.Open(recipeDto, "Ta bort recept", recipeDto.Name);
    }

    private async Task HandleRecipeRemovalConfirmed(RecipeDto recipeDto)
    {
        if (RecipeService == null)
        {
            string errorMessage = "Recipe service is not available during ingredient removal.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (NavigationManager == null)
        {
            string errorMessage = "Cannot navigate to index page after recipe removal because navigation manager has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (ToastService == null)
        {
            string errorMessage = "Cannot remove recipe because toast service has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        bool removalFromDatabaseSuccessful = await RecipeService.DeleteAsync(recipeDto.Id);

        if (removalFromDatabaseSuccessful)
        {
            ToastService.ShowInfo("Recept borttagen.");

            NavigationManager.NavigateTo("/");
        }
    }

    private bool ContainsInstructions()
    {
        if (_recipeDto == null)
        {
            string errorMessage = "Cannot access recipe instructions because recipe has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        return string.IsNullOrWhiteSpace(_recipeDto.Instructions) == false;
    }

    private string GetMeasurement(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (ingredientMeasurementDto == null)
        {
            string errorMessage = "Cannot access ingredient measurement within recipe because passed ingredient measurement has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        return ingredientMeasurementDto.Measurement.ToString() + " " + ingredientMeasurementDto.MeasurementUnit.ToSymbol();
    }

    private string GetIngredientMeasurementRowClass(CheckableIngredientMeasurement checkableIngredientMeasurement)
    {
        if (checkableIngredientMeasurement == null)
        {
            string errorMessage = "Cannot set class on ingredient measurement table row because checkable ingredient measurement has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        return checkableIngredientMeasurement.IsChecked ? "table-primary" : "";
    }

    private class CheckableIngredientMeasurement
    {
        public bool IsChecked { get; set; } = false;
        public IngredientMeasurementDto IngredientMeasurementDto { get; set; } = new();
    }
}
