using BlazingRecept.Client.Components.Utilities;
using BlazingRecept.Client.Pages;
using BlazingRecept.Client.Utilities;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Client.Components.PageComponents.CreateRecipePage;

public partial class AddIngredientMeasurementModal : ComponentBase
{
    private readonly string _editFormId = "AddIngredientMeasurementModalEditForm";

    private Modal? _modal;
    private CustomValidation? _customValidation;
    private EditContext? _editContext;

    private Form _form = new();

    [CascadingParameter]
    public CreateRecipe? CreateRecipe { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _editContext = new(_form);
    }

    public void Open(IngredientMeasurementDto? ingredientMeasurementDto)
    {
        if (_modal == null) throw new InvalidOperationException("Modal cannot be opened because it has not been set.");

        if (ingredientMeasurementDto != null)
        {
            _form = new()
            {
                IngredientDto = ingredientMeasurementDto.IngredientDto,
                Measurement = ingredientMeasurementDto.Measurement,
                MeasurementUnit = ingredientMeasurementDto.MeasurementUnit,
                Grams = ingredientMeasurementDto.Grams.ToString(),
                Note = ingredientMeasurementDto.Note,
                SortOrder = ingredientMeasurementDto.SortOrder,
            };
        }
        else
        {
            _form = new();
        }

        _editContext = new(_form);
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
        if (CreateRecipe == null) throw new InvalidOperationException();

        if (Validate())
        {
            if (IngredientAlreadyAdded(_form.IngredientDto))
            {
                EditExistingIngredient();
            }
            else
            {
                AddNewIngredient();
            }

            CreateRecipe.Refresh();
            _modal.Close();
        }
    }

    private void EditExistingIngredient()
    {
        if (CreateRecipe == null) throw new InvalidOperationException();

        IngredientMeasurementDto? ingredientMeasurementDto = CreateRecipe.ContainedIngredientMeasurements
            .FirstOrDefault(ingredientMeasurement => ingredientMeasurement.IngredientDto.Id == _form.IngredientDto.Id);

        if (ingredientMeasurementDto == null) throw new InvalidOperationException("Failed to find ingredient that was exepcted to exist edited recipe.");

        ingredientMeasurementDto.Measurement = _form.Measurement;
        ingredientMeasurementDto.MeasurementUnit = _form.MeasurementUnit;
        ingredientMeasurementDto.Grams = Convert.ToInt32(_form.Grams);
        ingredientMeasurementDto.Note = _form.Note;
    }

    private void AddNewIngredient()
    {
        if (CreateRecipe == null) throw new InvalidOperationException();

        int sortOrder = _form.SortOrder;

        if (sortOrder >= 0)
        {
            sortOrder = CreateRecipe.ContainedIngredientMeasurements.Count;
        }

        CreateRecipe.ContainedIngredientMeasurements.Add(new()
        {
            IngredientDto = _form.IngredientDto,
            Measurement = _form.Measurement,
            MeasurementUnit = _form.MeasurementUnit,
            Grams = Convert.ToInt32(_form.Grams),
            Note = _form.Note,
            SortOrder = sortOrder
        });
    }

    private bool Validate()
    {
        if (_customValidation == null) throw new InvalidOperationException("Custom validation is not set during validation.");
        if (_form == null) throw new InvalidOperationException("Form is not set during add ingredient modal validation.");

        _customValidation.ClearErrors();

        Dictionary<string, List<string>> errors = new();

        if (_form.IngredientDto.Id == Guid.Empty)
        {
            errors.Add(nameof(_form.IngredientDto), new List<string>() {
                "Ingredient is required."
            });
        }
        else if (IngredientAlreadyAdded(_form.IngredientDto))
        {
            errors.Add(nameof(_form.IngredientDto), new List<string>() {
                "Ingredient has already been added to recipe."
            });
        }

        if (string.IsNullOrWhiteSpace(_form.Measurement))
        {
            errors.Add(nameof(_form.Measurement), new List<string>() {
                "Measurement is required."
            });
        }

        if (string.IsNullOrWhiteSpace(_form.Grams))
        {
            errors.Add(nameof(_form.Grams), new List<string>() {
                "Grams is required."
            });
        }
        else if (int.TryParse(_form.Grams, out int basePortions) == false)
        {
            errors.Add(nameof(_form.Grams), new List<string>() {
                "Grams must only include numbers."
            });
        }
        else if (basePortions <= 0)
        {
            errors.Add(nameof(_form.Grams), new List<string>() {
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
            _form.IngredientDto = foundIngredients.First();
        }

        return await Task.FromResult(foundIngredients);
    }

    private bool IngredientAlreadyAdded(IngredientDto IngredientDto)
    {
        if (CreateRecipe == null) throw new InvalidOperationException();

        return CreateRecipe.ContainedIngredientMeasurements.Any(ingredientMeasurement => ingredientMeasurement.IngredientDto.Id == IngredientDto.Id);
    }

    public class Form
    {
        public IngredientDto IngredientDto { get; set; } = new();
        public string Measurement { get; set; } = string.Empty;
        public MeasurementUnit MeasurementUnit { get; set; } = MeasurementUnit.Gram;
        public string Grams { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public int SortOrder { get; set; } = -1;
    }
}
