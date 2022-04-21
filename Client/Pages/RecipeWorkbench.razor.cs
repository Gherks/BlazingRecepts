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
    private EditIngredientMeasurementModal? _editIngredientMeasurementModal;
    private RemovalConfirmationModal<IngredientDto>? _removalConfirmationModal;
    private CustomValidation? _customValidation;

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
                    PortionAmount = recipeDto.PortionAmount.ToString(),
                    CategoryDtoId = recipeDto.CategoryDto.Id,
                    Instructions = recipeDto.Instructions
                };

                ContainedIngredientMeasurements = recipeDto.IngredientMeasurementDtos;
            }
        }

        IsLoading = false;
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
            RecipeDto? recipeDto = ParseRecipeForm();

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

        if (string.IsNullOrWhiteSpace(_form.PortionAmount))
        {
            errors.Add(nameof(_form.PortionAmount), new List<string>() {
                "Antalet portioner m�ste anges."
            });
        }
        else if (int.TryParse(_form.PortionAmount, out int basePortions) == false)
        {
            errors.Add(nameof(_form.PortionAmount), new List<string>() {
                "Antalet portioner kan ej inneh�lla icke-numeriska tecken."
            });
        }
        else if (basePortions <= 0)
        {
            errors.Add(nameof(_form.PortionAmount), new List<string>() {
                "Antalet portioner m�ste vara en positiv siffra."
            });
        }

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

    private RecipeDto ParseRecipeForm()
    {
        if (_categoryDtos == null)
        {
            string errorMessage = "Cannot parse recipe form because it has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        CategoryDto? categoryDto = _categoryDtos.FirstOrDefault(categoryDto => categoryDto.Id == _form.CategoryDtoId);

        if (categoryDto == null)
        {
            string errorMessage = "Cannot parse recipe form because it contains an unknown category.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        RecipeDto recipeDto = new();

        recipeDto.Id = RecipeId;
        recipeDto.Name = _form.Name;
        recipeDto.PortionAmount = Convert.ToInt32(_form.PortionAmount);
        recipeDto.CategoryDto = categoryDto;
        recipeDto.Instructions = _form.Instructions;
        recipeDto.IngredientMeasurementDtos = ContainedIngredientMeasurements;

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

    public void HandleEditIngredientModalOpen(IngredientMeasurementDto? ingredientMeasurementDto)
    {
        if (_editIngredientMeasurementModal == null)
        {
            string errorMessage = "Cannot open edit ingredient modal because modal has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _editIngredientMeasurementModal.Open(ingredientMeasurementDto);
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
        return IsCreatingNewRecipe ? "Skapa recept" : "Editera recept";
    }

    private string GetConfirmationButtonLabel()
    {
        return IsCreatingNewRecipe ? "Skapa recept" : "Uppdatera recept";
    }

    private class Form
    {
        public string Name { get; set; } = string.Empty;
        public string PortionAmount { get; set; } = string.Empty;
        public Guid CategoryDtoId { get; set; } = Guid.Empty;
        public string Instructions { get; set; } = string.Empty;
        public string RecipeCreationErrorMessage { get; set; } = string.Empty;
    }
}
