using BlazingRecept.Client.Components.Common;
using BlazingRecept.Client.Pages.Base;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared;
using BlazingRecept.Shared.Dto;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Pages;

public partial class Recipe : PageBase
{
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
        Contracts.LogAndThrowWhenNull(RecipeService, "Cannot fetch recipe because recipe service is null.");

        IsLoading = true;

        await base.OnInitializedAsync();

        RecipeDto = await RecipeService.GetByIdAsync(RecipeId);

        IsLoading = false;
    }

    private void HandleNavigationToEditRecipe(RecipeDto recipeDto)
    {
        Contracts.LogAndThrowWhenNull(NavigationManager, "Cannot navigate to recipe edit page because navigation manager has not been set.");

        NavigationManager.NavigateTo($"recipeworkbench/{recipeDto.Id}");
    }

    private async Task HandleRecipeRemovalModalOpen(RecipeDto recipeDto)
    {
        Contracts.LogAndThrowWhenNull(_removalConfirmationModal, "Confirmation modal cannot be opened because it has not been set.");
        Contracts.LogAndThrowWhenNull(recipeDto, "Cannot start recipe removal process because passed recipe was not valid.");

        await _removalConfirmationModal.Open(recipeDto, "Ta bort recept", recipeDto.Name);
    }

    private async Task HandleRecipeRemovalConfirmed(RecipeDto recipeDto)
    {
        Contracts.LogAndThrowWhenNull(RecipeService, "Recipe service is not available during ingredient removal.");
        Contracts.LogAndThrowWhenNull(NavigationManager, "Cannot navigate to index page after recipe removal because navigation manager has not been set.");

        bool removalFromDatabaseSuccessful = await RecipeService.DeleteAsync(recipeDto.Id);

        if (removalFromDatabaseSuccessful)
        {
            MessengerService.AddInformation("Recept", "Recept borttagen.");
            NavigationManager.NavigateTo("/");
        }
    }

    private bool ContainsInstructions()
    {
        Contracts.LogAndThrowWhenNull(RecipeDto, "Cannot access recipe instructions because recipe has not been set.");

        return string.IsNullOrWhiteSpace(RecipeDto.Instructions) == false;
    }
}
