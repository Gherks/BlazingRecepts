using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Components.Utilities;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace BlazingRecept.Client.Pages;

public partial class Recipe : PageComponentBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "RecipePage";

    private RemovalConfirmationModal<RecipeDto>? _removalConfirmationModal;

    public RecipeDto? RecipeDto { get; set; } = new RecipeDto();

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

        RecipeDto = await RecipeService.GetByIdAsync(RecipeId);

        if (RecipeDto == null)
        {
            string errorMessage = "Cannot properly present recipe because sought recipe coulnd't be fetched.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
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
        if (RecipeDto == null)
        {
            string errorMessage = "Cannot access recipe instructions because recipe has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        return string.IsNullOrWhiteSpace(RecipeDto.Instructions) == false;
    }
}
