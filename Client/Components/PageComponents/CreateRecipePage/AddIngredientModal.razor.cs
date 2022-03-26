using BlazingRecept.Client.Components.Utilities;
using BlazingRecept.Client.Pages;
using BlazingRecept.Client.Utilities;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazingRecept.Client.Components.PageComponents.CreateRecipePage;

public partial class AddIngredientModal : ComponentBase
{
    private readonly string _editFormId = "AddIngredientModalEditForm";

    private Modal? _modal;
    private CustomValidation? _customValidation;
    private EditContext? _editContext;

    private IngredientForm _ingredientForm = new();

    [CascadingParameter]
    public CreateRecipe? CreateRecipe { get; set; }

    [Parameter]
    public Action<IngredientForm>? OnIngredientValid { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _editContext = new(_ingredientForm);
    }

    public void Open(IngredientForm? ingredientForm)
    {
        if (_modal == null) throw new InvalidOperationException("Modal cannot be opened because it has not been set.");

        if (ingredientForm != null)
        {
            _ingredientForm = ingredientForm;
        }
        else
        {
            _ingredientForm = new();
        }

        _editContext = new(_ingredientForm);

        _modal.Open();
    }

    private void HandleCancel()
    {
        if (_modal == null) throw new InvalidOperationException("Modal cannot be closed because it has not been set.");

        _modal.Close();
    }

    private void HandleValidFormSubmitted()
    {
        if (_modal == null) throw new InvalidOperationException();
        if (OnIngredientValid == null) throw new InvalidOperationException();

        if (Validate())
        {
            OnIngredientValid.Invoke(_ingredientForm);
            _modal.Close();
        }
    }

    public bool Validate()
    {
        if (_customValidation == null) throw new InvalidOperationException("Custom validation is not set during validation.");
        if (_ingredientForm == null) throw new InvalidOperationException("Form is not set during add ingredient modal validation.");

        _customValidation.ClearErrors();

        Dictionary<string, List<string>> errors = new();

        if (_ingredientForm.IngredientDto.Id == Guid.Empty)
        {
            errors.Add(nameof(_ingredientForm.IngredientDto), new List<string>() {
                "Ingredient is required."
            });
        }

        if (string.IsNullOrWhiteSpace(_ingredientForm.Measurement))
        {
            errors.Add(nameof(_ingredientForm.Measurement), new List<string>() {
                "Measurement is required."
            });
        }

        if (string.IsNullOrWhiteSpace(_ingredientForm.Grams))
        {
            errors.Add(nameof(_ingredientForm.Grams), new List<string>() {
                "Grams is required."
            });
        }
        else if (int.TryParse(_ingredientForm.Grams, out int basePortions) == false)
        {
            errors.Add(nameof(_ingredientForm.Grams), new List<string>() {
                "Grams must only include numbers."
            });
        }
        else if (basePortions <= 0)
        {
            errors.Add(nameof(_ingredientForm.Grams), new List<string>() {
                "Grams must be a positive number."
            });
        }

        if (errors.Count > 0)
        {
            _customValidation.DisplayErrors(errors);
            return false;
        }

        StateHasChanged();
        return true;
    }

    private async Task<IEnumerable<IngredientDto>> SearchForIngredients(string searchTerm)
    {
        if (CreateRecipe == null) throw new InvalidOperationException();
        if (CreateRecipe.Ingredients == null) throw new InvalidOperationException();

        List<IngredientDto> foundIngredients = CreateRecipe.Ingredients.Where(ingredientDto => ingredientDto.Name.ToLower().Contains(searchTerm.ToLower())).ToList();

        if (foundIngredients.Count == 1)
        {
            _ingredientForm.IngredientDto = foundIngredients.First();
        }

        return await Task.FromResult(foundIngredients);
    }
}
