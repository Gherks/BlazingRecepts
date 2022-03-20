using BlazingRecept.Client.Components.Utilities.Modals;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.PageComponents.IngredientPage;

public partial class RemovalConfirmationModal : ComponentBase
{
    private Modal? _modal { get; set; }

    private IngredientDto? _ingredientDto = null;

    [Inject]
    public IIngredientService? IngredientService { get; set; }

    [Parameter]
    public Func<IngredientDto, Task>? OnConfirm { get; set; }

    public void Open(IngredientDto ingredientDto)
    {
        if (_modal == null) throw new InvalidOperationException("Modal can not be opened because it has not been set.");

        _ingredientDto = ingredientDto;
        _modal.Open();
    }

    private async Task Confirm()
    {
        if (_modal == null) throw new InvalidOperationException("Modal can not be closed because it has not been set.");
        if (IngredientService == null) throw new InvalidOperationException("Ingredient service can not be used because it has not been set.");
        if (_ingredientDto == null) throw new InvalidOperationException("IngredientDto can not be removed because it has not been set.");
        if (OnConfirm == null) throw new InvalidOperationException("Can not call the on confirm routine because it has not been set.");

        await IngredientService.DeleteAsync(_ingredientDto.Id);

        await OnConfirm.Invoke(_ingredientDto);
        await _modal.CloseAsync();
    }

    private async Task Cancel()
    {
        if (_modal == null) throw new InvalidOperationException("Modal can not be closed because it has not been set.");

        await _modal.CloseAsync();
    }

    private string GetIngredientName()
    {
        if (_ingredientDto == null)
        {
            return "";
        }

        return _ingredientDto.Name;
    }
}
