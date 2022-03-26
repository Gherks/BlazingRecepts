using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.Utilities;

public partial class IngredientRemovalConfirmationModal : ComponentBase
{
    private Modal? _modal;

    private IngredientDto? _ingredientDto = null;

    [Parameter]
    public Func<IngredientDto, Task>? OnConfirm { get; set; }

    public void Open(IngredientDto ingredientDto)
    {
        if (_modal == null) throw new InvalidOperationException("Modal cannot be opened because it has not been set.");

        _ingredientDto = ingredientDto;
        _modal.Open();
    }

    private void HandleConfirm()
    {
        if (_modal == null) throw new InvalidOperationException("Modal cannot be closed because it has not been set.");
        if (OnConfirm == null) throw new InvalidOperationException("Cannot call the on confirm routine because it has not been set.");
        if (_ingredientDto == null) throw new InvalidOperationException("IngredientDto cannot be removed because it has not been set.");

        OnConfirm.Invoke(_ingredientDto);
        _modal.Close();
    }

    private void HandleCancel()
    {
        if (_modal == null) throw new InvalidOperationException("Modal cannot be closed because it has not been set.");

        _modal.Close();
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
