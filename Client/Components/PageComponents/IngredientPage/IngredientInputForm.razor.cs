using BlazingRecept.Client.Pages;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Client.Utilities;
using BlazingRecept.Shared.Dto;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.PageComponents.IngredientPage;

public partial class IngredientInputForm : ComponentBase
{
    private readonly Form _form = new();

    private CustomValidation? _customValidation;

    private IReadOnlyList<IngredientCategoryDto>? _ingredientCategoryDtos = new List<IngredientCategoryDto>();

    [CascadingParameter]
    protected internal Ingredients? IngredientsPage { get; private set; }

    [Inject]
    public IIngredientService? IngredientService { get; set; }

    [Inject]
    public IIngredientCategoryService? IngredientCategoryService { get; set; }

    [Inject]
    public IToastService? ToastService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (IngredientCategoryService != null)
        {
            _ingredientCategoryDtos = await IngredientCategoryService.GetAllAsync();
        }
    }

    private async Task HandleValidFormSubmitted()
    {
        if (IngredientService == null)
        {
            throw new InvalidOperationException();
        }

        if (await Validate())
        {
            IngredientDto newIngredientDto = CreateIngredientDtoFromForm() ?? throw new InvalidOperationException();

            IngredientDto? ingredientDto = await IngredientService.SaveAsync(newIngredientDto);

            if (ToastService == null) throw new InvalidOperationException();

            if (ingredientDto != null && ingredientDto.Id != Guid.Empty)
            {
                if (IngredientsPage == null) throw new InvalidOperationException();

                ToastService.ShowSuccess("Ingredient successfully added!");
                IngredientsPage.AddNewIngredientToCollection(ingredientDto);
            }
            else
            {
                ToastService.ShowError("Failed to add new ingredient.");
            }
        }
    }

    private async Task<bool> Validate()
    {
        if (_customValidation == null) throw new InvalidOperationException("Custom validation object is not available during validation.");

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
            if (IngredientService == null) throw new InvalidOperationException("Ingredient service is not available during validation.");

            bool ingredientExists = await IngredientService.AnyAsync(_form.Name);

            if (ingredientExists)
            {
                errors.Add(nameof(_form.IngredientCreationErrorMessage), new List<string>() {
                    "Ingredient with name already exists."
                });
            }
        }

        ValidateStringInt(_form.Fat, nameof(_form.Fat), errors);
        ValidateStringInt(_form.Carbohydrates, nameof(_form.Carbohydrates), errors);
        ValidateStringInt(_form.Protein, nameof(_form.Protein), errors);
        ValidateStringInt(_form.Calories, nameof(_form.Calories), errors);

        if (_form.IngredientCategoryDtoId == Guid.Empty)
        {
            errors.Add(nameof(_form.IngredientCategoryDtoId), new List<string>() {
                "Ingredient category is required."
            });
        }

        if (errors.Count > 0)
        {
            _customValidation.DisplayErrors(errors);
            return false;
        }

        return true;
    }

    private void ValidateStringInt(string variableValue, string variableName, Dictionary<string, List<string>> errors)
    {
        if (string.IsNullOrWhiteSpace(variableValue))
        {
            errors.Add(variableName, new List<string>() {
                $"{variableName} is required."
            });
        }
        else if (int.TryParse(variableValue, out int intValue) == false)
        {
            errors.Add(variableName, new List<string>() {
                $"{variableName} must only include numbers."
            });
        }
        else if (intValue <= 0)
        {
            errors.Add(variableName, new List<string>() {
                $"{variableName} must be a positive number."
            });
        }
    }

    private IngredientDto? CreateIngredientDtoFromForm()
    {
        if (_ingredientCategoryDtos == null)
        {
            throw new InvalidOperationException();
        }

        IngredientCategoryDto? ingredientCategoryDto = _ingredientCategoryDtos.FirstOrDefault(ingredientCategoryDto => ingredientCategoryDto.Id == _form.IngredientCategoryDtoId);

        if (ingredientCategoryDto == null)
        {
            throw new InvalidOperationException();
        }

        return new IngredientDto()
        {
            Name = _form.Name,
            Fat = double.Parse(_form.Fat),
            Carbohydrates = double.Parse(_form.Carbohydrates),
            Protein = double.Parse(_form.Protein),
            Calories = double.Parse(_form.Calories),
            CategoryDto = ingredientCategoryDto
        };
    }

    private class Form
    {
        public string Name { get; set; } = string.Empty;
        public string Fat { get; set; } = string.Empty;
        public string Carbohydrates { get; set; } = string.Empty;
        public string Protein { get; set; } = string.Empty;
        public string Calories { get; set; } = string.Empty;
        public Guid IngredientCategoryDtoId { get; set; } = Guid.Empty;
        public string IngredientCreationErrorMessage { get; set; } = string.Empty;
    }
}
