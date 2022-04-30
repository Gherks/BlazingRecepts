using BlazingRecept.Client.Components.Common;
using BlazingRecept.Client.Pages.Base;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace BlazingRecept.Client.Pages;

public partial class Recipe : PageBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "RecipePage";

    private RemovalConfirmationModal<RecipeDto>? _removalConfirmationModal;

    public RecipeDto? RecipeDto { get; private set; } = new RecipeDto();

    [Parameter]
    public Guid RecipeId { get; set; }

    [Inject]
    protected internal IRecipeService? RecipeService { get; private set; }

    [Inject]
    protected internal IHxMessengerService? MessengerService { get; private set; }

    [Inject]
    protected internal NavigationManager? NavigationManager { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        await base.OnInitializedAsync();

        if (RecipeService == null)
        {
            const string errorMessage = "Cannot fetch recipe because recipe service is null.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        RecipeDto = await RecipeService.GetByIdAsync(RecipeId);

        if (RecipeDto == null)
        {
            const string errorMessage = "Cannot properly present recipe because sought recipe coulnd't be fetched.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        IsLoading = false;
    }

    private void HandleNavigationToEditRecipe(RecipeDto recipeDto)
    {
        if (NavigationManager == null)
        {
            const string errorMessage = "Cannot navigate to recipe edit page because navigation manager has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        NavigationManager.NavigateTo($"recipeworkbench/{recipeDto.Id}");
    }

    private void HandleRecipeRemovalModalOpen(RecipeDto recipeDto)
    {
        if (_removalConfirmationModal == null)
        {
            const string errorMessage = "Confirmation modal cannot be opened because it has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (recipeDto == null)
        {
            const string errorMessage = "Cannot start recipe removal process because passed recipe was not valid.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _removalConfirmationModal.Open(recipeDto, "Ta bort recept", recipeDto.Name);
    }

    private async Task HandleRecipeRemovalConfirmed(RecipeDto recipeDto)
    {
        if (RecipeService == null)
        {
            const string errorMessage = "Recipe service is not available during ingredient removal.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (NavigationManager == null)
        {
            const string errorMessage = "Cannot navigate to index page after recipe removal because navigation manager has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        bool removalFromDatabaseSuccessful = await RecipeService.DeleteAsync(recipeDto.Id);

        if (removalFromDatabaseSuccessful)
        {
            MessengerService.AddInformation("Recept", "Recept borttagen.");
            NavigationManager.NavigateTo("/");
        }
    }

    private bool ContainsInstructions()
    {
        if (RecipeDto == null)
        {
            const string errorMessage = "Cannot access recipe instructions because recipe has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        return string.IsNullOrWhiteSpace(RecipeDto.Instructions) == false;
    }
}
