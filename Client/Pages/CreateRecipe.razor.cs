using BlazingRecept.Client.Components.PageComponents.CreateRecipePage;
using BlazingRecept.Client.Components.Utilities;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Client.Utilities;
using BlazingRecept.Shared.Dto;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Pages;

public partial class CreateRecipe : ComponentBase
{
    private Form _form = new();

    private AddIngredientMeasurementModal? _addIngredientMeasurementModal;
    private EditIngredientMeasurementModal? _editIngredientMeasurementModal;
    private IngredientRemovalConfirmationModal? _ingredientRemovalConfirmationModal;
    private CustomValidation? _customValidation;

    public IReadOnlyList<IngredientDto>? Ingredients { get; set; } = new List<IngredientDto>();
    public List<IngredientMeasurementDto> ContainedIngredientMeasurements { get; set; } = new();

    [Parameter]
    public Guid RecipeId { get; set; } = Guid.Empty;

    [Inject]
    public IIngredientService? IngredientService { get; set; }

    [Inject]
    public IRecipeService? RecipeService { get; set; }

    [Inject]
    public IToastService? ToastService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (IngredientService == null) throw new InvalidOperationException();

        Ingredients = await IngredientService.GetAllAsync();

        if (RecipeId != Guid.Empty)
        {
            if (RecipeService == null) throw new InvalidOperationException();

            RecipeDto? recipeDto = await RecipeService.GetByIdAsync(RecipeId);

            if(recipeDto != null)
            {
                _form = new()
                {
                    Name = recipeDto.Name,
                    PortionAmount = recipeDto.PortionAmount.ToString(),
                    Instructions = recipeDto.Instructions
                };

                ContainedIngredientMeasurements = recipeDto.IngredientMeasurementDtos;
            }
        }
    }

    public void Refresh()
    {
        StateHasChanged();
    }

    private async Task HandleValidFormSubmitted()
    {
        if (await Validate())
        {
            RecipeDto? recipeDto = ParseRecipeForm();

            if (RecipeService == null) throw new InvalidOperationException();

            recipeDto = await RecipeService.SaveAsync(recipeDto);

            if (recipeDto != null && recipeDto.Id != Guid.Empty)
            {
                if (ToastService == null) throw new InvalidOperationException();

                ToastService.ShowSuccess("Recipe successfully added!");

                _form = new();
                ContainedIngredientMeasurements.Clear();

                StateHasChanged();
            }
        }
    }

    private async Task<bool> Validate()
    {
        if (_customValidation == null) throw new InvalidOperationException("Custom validation variable is not set during validation.");

        _customValidation.ClearErrors();

        Dictionary<string, List<string>> errors = new();

        if (string.IsNullOrWhiteSpace(_form.Name))
        {
            errors.Add(nameof(_form.Name), new List<string>() {
                "Name is required."
            });
        }
        else
        {
            if (RecipeService == null) throw new InvalidOperationException("Ingredient service is not available during validation.");

            bool recipeExists = await RecipeService.AnyAsync(_form.Name);

            if (recipeExists)
            {
                errors.Add(nameof(_form.RecipeCreationErrorMessage), new List<string>() {
                    "Recipe with name already exists."
                });
            }
        }

        if (string.IsNullOrWhiteSpace(_form.PortionAmount))
        {
            errors.Add(nameof(_form.PortionAmount), new List<string>() {
                "Amount of portions is required."
            });
        }
        else if (int.TryParse(_form.PortionAmount, out int basePortions) == false)
        {
            errors.Add(nameof(_form.PortionAmount), new List<string>() {
                "Portions must only include numbers."
            });
        }
        else if (basePortions <= 0)
        {
            errors.Add(nameof(_form.PortionAmount), new List<string>() {
                "Portions must be a positive number."
            });
        }

        if (errors.Count > 0)
        {
            _customValidation.DisplayErrors(errors);
            return false;
        }

        return true;
    }

    private RecipeDto ParseRecipeForm()
    {
        RecipeDto recipeDto = new();

        recipeDto.Name = _form.Name;
        recipeDto.PortionAmount = Convert.ToInt32(_form.PortionAmount);
        recipeDto.Instructions = _form.Instructions;
        recipeDto.IngredientMeasurementDtos = ContainedIngredientMeasurements;

        for (int index = 0; index < recipeDto.IngredientMeasurementDtos.Count; ++index)
        {
            recipeDto.IngredientMeasurementDtos[index].SortOrder = index;
        }

        return recipeDto;
    }

    public void HandleAddIngredientModalOpen(IngredientMeasurementDto? ingredientMeasurementDto)
    {
        if (_addIngredientMeasurementModal == null) throw new InvalidOperationException();

        _addIngredientMeasurementModal.Open(ingredientMeasurementDto);
    }

    public void HandleEditIngredientModalOpen(IngredientMeasurementDto? ingredientMeasurementDto)
    {
        if (_editIngredientMeasurementModal == null) throw new InvalidOperationException();

        _editIngredientMeasurementModal.Open(ingredientMeasurementDto);
    }

    public void OpenIngredientRemovalModalOpen(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (_ingredientRemovalConfirmationModal == null) throw new InvalidOperationException();

        _ingredientRemovalConfirmationModal.Open(ingredientMeasurementDto.IngredientDto);
    }

    public Task HandleIngredientRemovalConfirmed(IngredientDto ingredientDto)
    {
        IngredientMeasurementDto? ingredientMeasurementDto = ContainedIngredientMeasurements.FirstOrDefault(ingredientMeasurementDto =>
            ingredientMeasurementDto.IngredientDto.Id == ingredientDto.Id);

        if (ingredientMeasurementDto != null)
        {
            ContainedIngredientMeasurements.Remove(ingredientMeasurementDto);
            StateHasChanged();
        }
        else
        {
            // Log.Warning("Could not remove ingreident from recipe that was supposed to be removed.");
        }

        return Task.CompletedTask;
    }

    private string GetConfirmationButtonLabel()
    {
        return RecipeId == Guid.Empty ? "Add recipe" : "Update recipe";
    }

    private class Form
    {
        public string Name { get; set; } = string.Empty;
        public string PortionAmount { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public string RecipeCreationErrorMessage { get; set; } = string.Empty;
    }
}
