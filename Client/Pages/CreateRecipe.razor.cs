using BlazingRecept.Client.Components.PageComponents.CreateRecipePage;
using BlazingRecept.Client.Components.Utilities;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Client.Utilities;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Pages;

public partial class CreateRecipe : ComponentBase
{
    private readonly Form _form = new();


    private AddIngredientModal? _addIngredientModal;
    private IngredientRemovalConfirmationModal? _ingredientRemovalConfirmationModal;
    private CustomValidation? _customValidation;

    public IReadOnlyList<IngredientDto>? Ingredients { get; set; } = new List<IngredientDto>();
    public List<IngredientForm> IngredientForms { get; set; } = new();

    [Inject]
    public IIngredientService? IngredientService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (IngredientService == null) throw new InvalidOperationException("Add ingredient modal cannot be opened because ingredient service has not been set.");

        Ingredients = await IngredientService.GetAllAsync();
    }

    private void HandleValidFormSubmitted()
    {
        if (Validate() == false)
        {
            return;
        }
    }

    private bool Validate()
    {
        if (_customValidation == null)
        {
            throw new InvalidOperationException("Custom validation variable is not set during validation.");
        }

        _customValidation.ClearErrors();

        Dictionary<string, List<string>> errors = new();

        if (string.IsNullOrWhiteSpace(_form.Name))
        {
            errors.Add(nameof(_form.Name), new List<string>() {
                "Name is required."
            });
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

    public void HandleAddIngredientModalOpen(IngredientForm? ingredientForm)
    {
        if (_addIngredientModal == null) throw new InvalidOperationException();

        _addIngredientModal.Open(ingredientForm);
    }

    public void OpenIngredientRemovalModalOpen(IngredientForm ingredientForm)
    {
        if (_ingredientRemovalConfirmationModal == null) throw new InvalidOperationException();

        _ingredientRemovalConfirmationModal.Open(ingredientForm.IngredientDto);
    }

    public Task HandleIngredientRemovalConfirmed(IngredientDto ingredientDto)
    {
        IngredientForm? ingredientForm = IngredientForms.FirstOrDefault(ingredientForm =>
            ingredientForm.IngredientDto.Id == ingredientDto.Id);

        if (ingredientForm != null)
        {
            IngredientForms.Remove(ingredientForm);
            StateHasChanged();
        }
        else
        {
            // Log.Warning("Could not remove ingreident from recipe that was supposed to be removed.");
        }

        return Task.CompletedTask;
    }

    private void HandleNewIngredientFromModal(IngredientForm newIngredientForm)
    {
        IngredientForm? existingIngredientForm = IngredientForms.FirstOrDefault(ingredientForm =>
            ingredientForm.IngredientDto.Name == newIngredientForm.IngredientDto.Name);

        if (existingIngredientForm != null)
        {
            int index = IngredientForms.IndexOf(newIngredientForm);
            IngredientForms[index] = newIngredientForm;
        }
        else
        {
            IngredientForms.Add(newIngredientForm);
        }

        StateHasChanged();
    }

    private class Form
    {
        public string Name { get; set; } = string.Empty;
        public string PortionAmount { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
    }
}
