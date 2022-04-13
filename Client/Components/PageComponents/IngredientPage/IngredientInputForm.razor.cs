using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Pages;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Client.Utilities;
using BlazingRecept.Shared.Dto;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Serilog;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Client.Components.PageComponents.IngredientPage;

public partial class IngredientInputForm : PageComponentBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "IngredientInputForm";

    private Form _form = new();

    private CustomValidation? _customValidation;

    private IReadOnlyList<CategoryDto>? _categoryDtos = new List<CategoryDto>();

    [CascadingParameter]
    protected internal Ingredients? IngredientsPage { get; private set; }

    [Inject]
    public IIngredientService? IngredientService { get; set; }

    [Inject]
    public ICategoryService? CategoryService { get; set; }

    [Inject]
    public IToastService? ToastService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        await base.OnInitializedAsync();

        if (CategoryService != null)
        {
            _categoryDtos = await CategoryService.GetAllOfTypeAsync(CategoryType.Ingredient);
        }

        IsLoading = false;
    }

    private async Task HandleNameBlur()
    {
        if (_customValidation == null)
        {
            string errorMessage = "Custom validation object is not available during blur validation.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (IngredientService == null)
        {
            string errorMessage = "Ingredient service is not available during blur validation.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        Dictionary<string, List<string>> errors = new();

        bool ingredientExists = await IngredientService.AnyAsync(_form.Name);

        if (ingredientExists)
        {
            errors.Add(nameof(_form.Name), new List<string>() {
                "Ingrediens med angivet namn finns redan."
            });
        }

        if (errors.Count > 0)
        {
            _customValidation.DisplayErrors(errors);
        }
    }

    private async Task HandleValidFormSubmitted()
    {
        if (IngredientService == null)
        {
            string errorMessage = "Ingredient service is not available during form validation.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (ToastService == null)
        {
            string errorMessage = "Toast service is not available during form validation.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (IngredientsPage == null)
        {
            string errorMessage = "Ingredient page reference is not available during form validation.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (await Validate())
        {
            IngredientDto newIngredientDto = CreateIngredientDtoFromForm() ?? throw new InvalidOperationException();
            IngredientDto? ingredientDto = await IngredientService.SaveAsync(newIngredientDto);

            if (ingredientDto != null && ingredientDto.Id != Guid.Empty)
            {
                ToastService.ShowSuccess("Ingrediens tillagd!");
                IngredientsPage.AddNewIngredientToCollection(ingredientDto);

                _form = new();
            }
            else
            {
                ToastService.ShowError("Kunde ej lägga till ingrediens.");
            }
        }
    }

    private async Task<bool> Validate()
    {
        if (_customValidation == null)
        {
            string errorMessage = "Custom validation object is not available during validation.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (IngredientService == null)
        {
            string errorMessage = "Ingredient service is not available during validation.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _customValidation.ClearErrors();

        Dictionary<string, List<string>> errors = new();

        if (string.IsNullOrWhiteSpace(_form.Name))
        {
            errors.Add(nameof(_form.Name), new List<string>() {
                "Namn måste anges."
            });
        }
        else
        {
            bool ingredientExists = await IngredientService.AnyAsync(_form.Name);

            if (ingredientExists)
            {
                errors.Add(nameof(_form.Name), new List<string>() {
                    "Ingrediens med angivet namn finns redan."
                });
            }
        }

        ValidateStringDouble(_form.Fat, nameof(_form.Fat), "Fett", errors);
        ValidateStringDouble(_form.Carbohydrates, nameof(_form.Carbohydrates), "Kolhydrater", errors);
        ValidateStringDouble(_form.Protein, nameof(_form.Protein), "Protein", errors);
        ValidateStringDouble(_form.Calories, nameof(_form.Calories), "Kalorier", errors);

        if (_form.CategoryDtoId == Guid.Empty)
        {
            errors.Add(nameof(_form.CategoryDtoId), new List<string>() {
                "Ingredienskategori måste anges."
            });
        }

        if (errors.Count > 0)
        {
            _customValidation.DisplayErrors(errors);
            return false;
        }

        return true;
    }

    private void ValidateStringDouble(string variableValue, string variableName, string displayName, Dictionary<string, List<string>> errors)
    {
        if (string.IsNullOrWhiteSpace(variableValue))
        {
            errors.Add(variableName, new List<string>() {
                $"{displayName} måste anges."
            });
        }
        else if (double.TryParse(variableValue, out double doubleValue) == false)
        {
            errors.Add(variableName, new List<string>() {
                $"{displayName} kan bara innehålla icke-numeriska tecken."
            });
        }
        else if (doubleValue < 0.0)
        {
            errors.Add(variableName, new List<string>() {
                $"{displayName} måste vara en positiv siffra."
            });
        }
    }

    private IngredientDto? CreateIngredientDtoFromForm()
    {
        if (_categoryDtos == null)
        {
            string errorMessage = ".";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        CategoryDto? categoryDto = _categoryDtos.FirstOrDefault(categoryDto => categoryDto.Id == _form.CategoryDtoId);

        if (categoryDto == null)
        {
            string errorMessage = ".";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        return new IngredientDto()
        {
            Name = _form.Name.Trim(),
            Fat = double.Parse(_form.Fat),
            Carbohydrates = double.Parse(_form.Carbohydrates),
            Protein = double.Parse(_form.Protein),
            Calories = double.Parse(_form.Calories),
            CategoryDto = categoryDto
        };
    }

    private class Form
    {
        public string Name { get; set; } = string.Empty;
        public string Fat { get; set; } = string.Empty;
        public string Carbohydrates { get; set; } = string.Empty;
        public string Protein { get; set; } = string.Empty;
        public string Calories { get; set; } = string.Empty;
        public Guid CategoryDtoId { get; set; } = Guid.Empty;
    }
}
