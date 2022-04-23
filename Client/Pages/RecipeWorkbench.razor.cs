using BlazingRecept.Client.Components.PageComponents.RecipeWorkbenchPage;
using BlazingRecept.Client.Components.Utilities;
using BlazingRecept.Client.Pages.Base;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Client.Utilities;
using BlazingRecept.Shared.Dto;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Serilog;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Client.Pages;

public partial class RecipeWorkbench : PageBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "RecipeWorkbenchPage";

    private Form _form = new();

    private AddIngredientMeasurementModal? _addIngredientMeasurementModal;
    private UpdateIngredientMeasurementModal? _updateIngredientMeasurementModal;
    private RemovalConfirmationModal<IngredientDto>? _removalConfirmationModal;
    private CustomValidation? _customValidation;
    private ElementReference _nameInput;
    private bool _shouldMoveFocusToNameElement = false;

    private IReadOnlyList<CategoryDto>? _categoryDtos = new List<CategoryDto>();

    public IReadOnlyList<IngredientDto>? Ingredients { get; private set; } = new List<IngredientDto>();

    public List<IngredientMeasurementDto> ContainedIngredientMeasurements { get; private set; } = new();

    [Parameter]
    public Guid RecipeId { get; set; } = Guid.Empty;

    [Inject]
    public IIngredientService? IngredientService { get; private set; }

    [Inject]
    public ICategoryService? CategoryService { get; private set; }

    [Inject]
    public IRecipeService? RecipeService { get; private set; }

    [Inject]
    public IToastService? ToastService { get; private set; }

    [Inject]
    public NavigationManager? NavigationManager { get; private set; }

    private bool IsCreatingNewRecipe => RecipeId == Guid.Empty;

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        if (RecipeService == null)
        {
            string errorMessage = "Cannot fetch recipe because recipe service is null.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (IngredientService == null)
        {
            string errorMessage = "Cannot fetch ingredients because ingredient service is null.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (CategoryService == null)
        {
            string errorMessage = "Cannot fetch categories because category service is null.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        Ingredients = await IngredientService.GetAllAsync();
        _categoryDtos = await CategoryService.GetAllOfTypeAsync(CategoryType.Recipe);

        if (RecipeId != Guid.Empty)
        {
            RecipeDto? recipeDto = await RecipeService.GetByIdAsync(RecipeId);

            if (recipeDto != null)
            {
                _form = new()
                {
                    Name = recipeDto.Name,
                    PortionAmount = recipeDto.PortionAmount,
                    CategoryDtoId = recipeDto.CategoryDto.Id,
                    Instructions = recipeDto.Instructions
                };

                ContainedIngredientMeasurements = recipeDto.IngredientMeasurementDtos;
            }
        }

        _shouldMoveFocusToNameElement = true;
        IsLoading = false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (_shouldMoveFocusToNameElement)
        {
            _shouldMoveFocusToNameElement = false;
            await _nameInput.FocusAsync();
        }
    }

    public void Refresh()
    {
        StateHasChanged();
    }

    private async Task HandleValidFormSubmitted()
    {
        if (RecipeService == null)
        {
            string errorMessage = "Cannot submit validated recipe because recipe service has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (NavigationManager == null)
        {
            string errorMessage = "Cannot submit validated recipe because navigation manager has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (ToastService == null)
        {
            string errorMessage = "Cannot submit validated recipe because toast service has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (await Validate())
        {
            RecipeDto? recipeDto = CreateRecipeDtoFromForm();

            recipeDto = await RecipeService.SaveAsync(recipeDto);

            if (recipeDto != null && recipeDto.Id != Guid.Empty)
            {
                string toastMessage = IsCreatingNewRecipe ? "Recept tillagd!" : "Recept uppdaterat!";
                ToastService.ShowSuccess(toastMessage);

                NavigationManager.NavigateTo($"recipe/{recipeDto.Id}");
            }
        }
    }

    private async Task<bool> Validate()
    {
        if (_customValidation == null)
        {
            string errorMessage = "Cannot validate recipe because custom validation object has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (RecipeService == null)
        {
            string errorMessage = "Cannot validate recipe because recipe service has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _customValidation.ClearErrors();

        Dictionary<string, List<string>> errors = new();

        if (string.IsNullOrWhiteSpace(_form.Name))
        {
            errors.Add(nameof(_form.Name), new List<string>() {
                "Namn m�ste anges."
            });
        }
        else if (IsCreatingNewRecipe)
        {
            bool recipeAlreadyExists = await RecipeService.AnyAsync(_form.Name);

            if (recipeAlreadyExists)
            {
                errors.Add(nameof(_form.RecipeCreationErrorMessage), new List<string>() {
                    "Recept med angivet namn finns redan."
                });
            }
        }

        InputValidation.ValidateNullableInt(_form.PortionAmount, nameof(_form.PortionAmount), "Antalet portioner", errors);

        if (_form.CategoryDtoId == Guid.Empty)
        {
            errors.Add(nameof(_form.CategoryDtoId), new List<string>() {
                "Kategori f�r recept m�ste anges."
            });
        }

        if (errors.Count > 0)
        {
            _customValidation.DisplayErrors(errors);
            return false;
        }

        return true;
    }

    private RecipeDto CreateRecipeDtoFromForm()
    {
        if (_categoryDtos == null)
        {
            string errorMessage = "Cannot create recipe dto from form because category list has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        CategoryDto? categoryDto = _categoryDtos.FirstOrDefault(categoryDto => categoryDto.Id == _form.CategoryDtoId);

        if (categoryDto == null)
        {
            string errorMessage = "Cannot create recipe dto from form because selected category in form does not exist.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (_form.PortionAmount == null)
        {
            string errorMessage = "Cannot create recipe dto from form because portion amount in form has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        RecipeDto recipeDto = new()
        {
            Id = RecipeId,
            Name = _form.Name,
            PortionAmount = _form.PortionAmount.Value,
            CategoryDto = categoryDto,
            Instructions = _form.Instructions,
            IngredientMeasurementDtos = ContainedIngredientMeasurements
        };

        for (int index = 0; index < recipeDto.IngredientMeasurementDtos.Count; ++index)
        {
            recipeDto.IngredientMeasurementDtos[index].SortOrder = index;
        }

        return recipeDto;
    }

    public void HandleAddIngredientModalOpen()
    {
        if (_addIngredientMeasurementModal == null)
        {
            string errorMessage = "Cannot open add ingredient modal because modal has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _addIngredientMeasurementModal.Open();
    }

    public void HandleUpdateIngredientModalOpen(IngredientMeasurementDto? ingredientMeasurementDto)
    {
        if (_updateIngredientMeasurementModal == null)
        {
            string errorMessage = "Cannot open update ingredient modal because modal has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _updateIngredientMeasurementModal.Open(ingredientMeasurementDto);
    }

    public void OpenIngredientRemovalModalOpen(IngredientMeasurementDto ingredientMeasurementDto)
    {
        if (_removalConfirmationModal == null)
        {
            string errorMessage = "Cannot open removal confirmation modal because modal has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _removalConfirmationModal.Open(ingredientMeasurementDto.IngredientDto, "Ta bort ingrediens", ingredientMeasurementDto.IngredientDto.Name);
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
            const string errorMessage = "Couldn't remove ingredient from edited recipe because sought ingredient doesn't exist in recipe.";
            Log.ForContext(_logProperty, _logDomainName).Warning(errorMessage);
        }

        return Task.CompletedTask;
    }

    private string GetTitle()
    {
        return IsCreatingNewRecipe ? "Skapa recept" : "Uppdatera recept";
    }

    private string GetConfirmationButtonLabel()
    {
        return IsCreatingNewRecipe ? "Skapa recept" : "Uppdatera recept";
    }

    private class Form
    {
        public string Name { get; set; } = string.Empty;
        public int? PortionAmount { get; set; } = null;
        public Guid CategoryDtoId { get; set; } = Guid.Empty;
        public string Instructions { get; set; } = string.Empty;
        public string RecipeCreationErrorMessage { get; set; } = string.Empty;
    }
}
