using BlazingRecept.Client.Components.Common;
using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Extensions;
using BlazingRecept.Client.Pages;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Client.Utilities;
using BlazingRecept.Contract;
using BlazingRecept.Shared.Dto;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Client.Components.PageComponents.IngredientPage;

public partial class IngredientInputForm : PageComponentBase
{
    private static readonly string _editFormId = "IngredientInputFormEditForm";

    private Form _form = new();

    private CustomValidation? _customValidation;
    private EditContext? _editContext;
    private ElementReference _nameInput;
    private ProcessingButton? _processingButtonSubmit;

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
    protected internal IHxMessengerService? MessengerService { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        await base.OnInitializedAsync();

        _editContext = new(_form);

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
        return _collapseIsShow ? "D?lj formul?r" : "Visa formul?r";
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

        Contracts.LogAndThrowWhenNull(_customValidation, "Custom validation object is not available during blur validation.");
        Contracts.LogAndThrowWhenNull(IngredientService, "Ingredient service is not available during blur validation.");

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
        Contracts.LogAndThrowWhenNull(_processingButtonSubmit, "Cannot add new ingredient because processing button has not been set.");

        _processingButtonSubmit.IsProcessing = true;

        Contracts.LogAndThrowWhenNull(IngredientService, "Cannot add new ingredient because ingredient service has not been set.");
        Contracts.LogAndThrowWhenNull(IngredientsPage, "Cannot add new ingredient because ingredient page reference has not been set.");

        if (await Validate())
        {
            IngredientDto newIngredientDto = CreateIngredientDtoFromForm();
            IngredientDto? ingredientDto = await IngredientService.SaveAsync(newIngredientDto);

            if (ingredientDto != null && ingredientDto.Id != Guid.Empty)
            {
                MessengerService.AddSuccess("Ingredienser", "Ingrediens tillagd!");
                IngredientsPage.AddNewIngredientToCollection(ingredientDto);

                _form = new();
                _shouldMoveFocusToNameElement = true;
            }
            else
            {
                MessengerService.AddError("Ingredienser", "Kunde ej l?gga till ingrediens.");
            }
        }

        _processingButtonSubmit.IsProcessing = false;
    }

    private async Task<bool> Validate()
    {
        Contracts.LogAndThrowWhenNull(_customValidation, "Custom validation object is not available during validation.");
        Contracts.LogAndThrowWhenNull(IngredientService, "Ingredient service is not available during validation.");

        _customValidation.ClearErrors();

        Dictionary<string, List<string>> errors = new();

        if (string.IsNullOrWhiteSpace(_form.Name))
        {
            errors.Add(nameof(_form.Name), new List<string>() {
                "Namn m?ste anges."
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
                "Ingredienskategori m?ste anges."
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
        Contracts.LogAndThrowWhenNull(_categoryDtos, "Cannot create ingredient dto from form because category list has not been set.");

        CategoryDto? categoryDto = _categoryDtos.FirstOrDefault(categoryDto => categoryDto.Id == _form.CategoryDtoId);

        Contracts.LogAndThrowWhenNull(categoryDto, "Cannot create ingredient dto from form because selected category in form does not exist.");
        Contracts.LogAndThrowWhenNull(_form.Fat, "Cannot create ingredient dto from form because fat in form has not been set.");
        Contracts.LogAndThrowWhenNull(_form.Carbohydrates, "Cannot create ingredient dto from form because carbohydrates in form has not been set.");
        Contracts.LogAndThrowWhenNull(_form.Protein, "Cannot create ingredient dto from form because protein in form has not been set.");
        Contracts.LogAndThrowWhenNull(_form.Calories, "Cannot create ingredient dto from form because calories in form has not been set.");

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
