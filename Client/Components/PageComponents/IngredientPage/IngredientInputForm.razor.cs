using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Pages;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Client.Utilities;
using BlazingRecept.Shared.Dto;
using Blazored.Toast.Services;
using Havit.Blazor.Components.Web.Bootstrap;
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
    private HxCollapse? _hxCollapse;
    private ElementReference _nameInput;

    private bool _collapseIsShow = false;
    private bool _shouldMoveFocusToNameElement = false;

    private IReadOnlyList<CategoryDto>? _categoryDtos = new List<CategoryDto>();

    [CascadingParameter]
    protected internal Ingredients? IngredientsPage { get; private set; }

    [Inject]
    protected internal IIngredientService? IngredientService { get; private set; }

    [Inject]
    protected internal ICategoryService? CategoryService { get; private set; }

    [Inject]
    public IToastService? ToastService { get; private set; }

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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (_shouldMoveFocusToNameElement)
        {
            _shouldMoveFocusToNameElement = false;
            await _nameInput.FocusAsync();
        }
    }

    private string GetCollapseToggleTitle()
    {
        return _collapseIsShow ? "Dölj formulär" : "Visa formulär";
    }

    private async Task HandleCollapseShown()
    {
        _collapseIsShow = true;
        await _nameInput.FocusAsync();
    }

    private void HandleCollapseHidden()
    {
        _collapseIsShow = false;
    }

    private async Task HandleNameBlur()
    {
        if (string.IsNullOrWhiteSpace(_form.Name))
        {
            return;
        }

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
            if (_customValidation.ContainsError(nameof(_form.Name)))
            {
                _customValidation.RemoveError(nameof(_form.Name));
            }

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
            IngredientDto newIngredientDto = CreateIngredientDtoFromForm();
            IngredientDto? ingredientDto = await IngredientService.SaveAsync(newIngredientDto);

            if (ingredientDto != null && ingredientDto.Id != Guid.Empty)
            {
                ToastService.ShowSuccess("Ingrediens tillagd!");
                IngredientsPage.AddNewIngredientToCollection(ingredientDto);

                _form = new();
                _shouldMoveFocusToNameElement = true;
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

        InputValidation.ValidateNullableDouble(_form.Fat, nameof(_form.Fat), "Fett", errors);
        InputValidation.ValidateNullableDouble(_form.Carbohydrates, nameof(_form.Carbohydrates), "Kolhydrater", errors);
        InputValidation.ValidateNullableDouble(_form.Protein, nameof(_form.Protein), "Protein", errors);
        InputValidation.ValidateNullableDouble(_form.Calories, nameof(_form.Calories), "Kalorier", errors);

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

    private IngredientDto CreateIngredientDtoFromForm()
    {
        if (_categoryDtos == null)
        {
            string errorMessage = "Cannot create ingredient dto from form because category list has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        CategoryDto? categoryDto = _categoryDtos.FirstOrDefault(categoryDto => categoryDto.Id == _form.CategoryDtoId);

        if (categoryDto == null)
        {
            string errorMessage = "Cannot create ingredient dto from form because selected category in form does not exist.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (_form.Fat == null)
        {
            string errorMessage = "Cannot create ingredient dto from form because fat in form has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (_form.Carbohydrates == null)
        {
            string errorMessage = "Cannot create ingredient dto from form because carbohydrates in form has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (_form.Protein == null)
        {
            string errorMessage = "Cannot create ingredient dto from form because protein in form has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (_form.Calories == null)
        {
            string errorMessage = "Cannot create ingredient dto from form because calories in form has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        return new()
        {
            Name = _form.Name.Trim(),
            Fat = _form.Fat.Value,
            Carbohydrates = _form.Carbohydrates.Value,
            Protein = _form.Protein.Value,
            Calories = _form.Calories.Value,
            CategoryDto = categoryDto
        };
    }

    private class Form
    {
        public string Name { get; set; } = string.Empty;
        public double? Fat { get; set; } = null;
        public double? Carbohydrates { get; set; } = null;
        public double? Protein { get; set; } = null;
        public double? Calories { get; set; } = null;
        public Guid CategoryDtoId { get; set; } = Guid.Empty;
    }
}
