using BlazingRecept.Client.Components.Utilities;
using BlazingRecept.Client.Pages.Base;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared;
using BlazingRecept.Shared.Dto;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Pages;

public partial class Recipe : PageBase
{
    private RecipeDto? _recipeDto = new RecipeDto();

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
        if (RecipeService == null) throw new InvalidOperationException();

        _recipeDto = await RecipeService.GetByIdAsync(RecipeId);
    }

    private void HandleNavigationToEditRecipe(RecipeDto recipeDto)
    {
        if (NavigationManager == null) throw new InvalidOperationException();

        NavigationManager.NavigateTo($"recipeworkbench/{recipeDto.Id}");
    }

    private void HandleIngredientRemovalModalOpen(RecipeDto recipeDto)
    {
        if (_removalConfirmationModal == null) throw new InvalidOperationException("ConfirmationModal cannot be opened because it has not been set.");
        if (recipeDto == null) throw new InvalidOperationException();

        _removalConfirmationModal.Open(recipeDto, "Remove recipe", recipeDto.Name);
    }

    private async Task HandleIngredientRemovalConfirmed(RecipeDto recipeDto)
    {
        if (RecipeService == null) throw new InvalidOperationException();
        if (NavigationManager == null) throw new InvalidOperationException();

        bool removalFromDatabaseSuccessful = await RecipeService.DeleteAsync(recipeDto.Id);

        if (removalFromDatabaseSuccessful)
        {
            if (ToastService == null) throw new InvalidOperationException();

            ToastService.ShowInfo("Successfully removed recipe.");

            NavigationManager.NavigateTo("/");
        }
    }

    private bool ContainsInstructions()
    {
        if (_recipeDto == null) throw new InvalidOperationException();

        return string.IsNullOrWhiteSpace(_recipeDto.Instructions) == false;
    }

    private string GetMeasurement(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (ingredientMeasurementDto == null) throw new InvalidOperationException();

        return ingredientMeasurementDto.Measurement.ToString() + " " + ingredientMeasurementDto.MeasurementUnit.ToSymbol();
    }
}
