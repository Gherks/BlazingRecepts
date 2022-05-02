using BlazingRecept.Client.Components.Common;
using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Pages;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using BlazingRecept.Shared.Extensions;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace BlazingRecept.Client.Components.PageComponents.DailyIntakePage;

public partial class DailyIntakeTable : PageComponentBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "DailyIntakeTable";

    private DailyIntakeEntryDto? _uneditedDailyIntakeEntryDto = null;

    [Parameter]
    public Guid CollectionId { get; set; }

    [Parameter]
    public List<DailyIntakeEntryDto>? DailyIntakeEntryDtos { get; set; }

    [Parameter]
    public Func<DailyIntakeEntryDto, Task<bool>>? OnDailyIntakeEntryMoveUpInOrderAsync { get; set; }

    [Parameter]
    public Func<DailyIntakeEntryDto, Task<bool>>? OnDailyIntakeEntryMoveDownInOrderAsync { get; set; }

    [Parameter]
    public Func<DailyIntakeEntryDto, Task<bool>>? OnDailyIntakeEntryEditSubmitAsync { get; set; }

    [Parameter]
    public Func<DailyIntakeEntryDto, Task<bool>>? OnDailyIntakeEntryRemoveAsync { get; set; }

    [Parameter]
    public Func<Guid, Task<bool>>? OnDailyIntakeEntryAddAsync { get; set; }

    private async Task HandleDailyIntakeEntryMoveUpInOrderClickAsync(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        if (OnDailyIntakeEntryMoveUpInOrderAsync != null)
        {
            await OnDailyIntakeEntryMoveUpInOrderAsync.Invoke(dailyIntakeEntryDto);
        }
    }

    private async Task HandleDailyIntakeEntryMoveDownInOrderClickAsync(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        if (OnDailyIntakeEntryMoveDownInOrderAsync != null)
        {
            await OnDailyIntakeEntryMoveDownInOrderAsync.Invoke(dailyIntakeEntryDto);
        }
    }

    private async Task HandleDailyIntakeEntryEditSubmitClickAsync(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        if (OnDailyIntakeEntryEditSubmitAsync != null)
        {
            bool successfullyEdited = await OnDailyIntakeEntryEditSubmitAsync.Invoke(dailyIntakeEntryDto);

            if(successfullyEdited)
            {
                _uneditedDailyIntakeEntryDto = null;
            }
        }
    }

    private async Task HandleDailyIntakeEntryRemoveClickAsync(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        if (OnDailyIntakeEntryRemoveAsync != null)
        {
            await OnDailyIntakeEntryRemoveAsync.Invoke(dailyIntakeEntryDto);
        }
    }

    private async Task HandleDailyIntakeEntryAddClickAsync()
    {
        if (OnDailyIntakeEntryAddAsync != null)
        {
            await OnDailyIntakeEntryAddAsync.Invoke(CollectionId);
        }
    }

    private void HandleDailyIntakeEntryEditClick(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        _uneditedDailyIntakeEntryDto = new()
        {
            Id = dailyIntakeEntryDto.Id,
            ProductName = dailyIntakeEntryDto.ProductName,
            Amount = dailyIntakeEntryDto.Amount,
            Fat = dailyIntakeEntryDto.Fat,
            Carbohydrates = dailyIntakeEntryDto.Carbohydrates,
            Protein = dailyIntakeEntryDto.Protein,
            Calories = dailyIntakeEntryDto.Calories,
            ProteinPerCalorie = dailyIntakeEntryDto.ProteinPerCalorie,
            SortOrder = dailyIntakeEntryDto.SortOrder,
            IsRecipe = dailyIntakeEntryDto.IsRecipe,
            ProductId = dailyIntakeEntryDto.ProductId,
            CollectionId = dailyIntakeEntryDto.CollectionId
        };

        StateHasChanged();
    }

    private void HandleDailyIntakeEntryEditCancelClick(DailyIntakeEntryDto editedDailyIntakeEntryDto)
    {
        if (_uneditedDailyIntakeEntryDto == null)
        {
            const string errorMessage = "Cannot cancel daily intake entry table inline editing because unedited daily intake entry dto is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        editedDailyIntakeEntryDto.Amount = _uneditedDailyIntakeEntryDto.Amount;
        _uneditedDailyIntakeEntryDto = null;

        StateHasChanged();
    }

    private string GetAmount(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        if (dailyIntakeEntryDto.IsRecipe)
        {
            string portionSuffix = (dailyIntakeEntryDto.Amount > 1) ? "portioner" : "portion";

            return $"{Math.Round(dailyIntakeEntryDto.Amount, 2)} {portionSuffix}";
        }
        else
        {
            return $"{Math.Round(dailyIntakeEntryDto.Amount, 2)} gram";
        }
    }

    private double GetFat(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        return Math.Round(dailyIntakeEntryDto.Fat, 2);
    }

    private double GetCarbohydrates(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        return Math.Round(dailyIntakeEntryDto.Carbohydrates, 2);
    }

    private double GetProtein(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        return Math.Round(dailyIntakeEntryDto.Protein, 2);
    }

    private double GetCalories(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        return Math.Round(dailyIntakeEntryDto.Calories, 2);
    }

    private double GetProteinPerCalorie(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        return Math.Round(dailyIntakeEntryDto.ProteinPerCalorie, 2);
    }

    private double GetFatTotal(List<DailyIntakeEntryDto> dailyIntakeEntryDtos)
    {
        if (dailyIntakeEntryDtos == null)
        {
            const string errorMessage = "Cannot present daily intake grams total because daily intake list is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        double fatTotal = dailyIntakeEntryDtos.Sum(dailyIntakeEntryDto => dailyIntakeEntryDto.Fat);

        return Math.Round(fatTotal, 2);
    }

    private double GetCarbohydrateTotal(List<DailyIntakeEntryDto> dailyIntakeEntryDtos)
    {
        if (dailyIntakeEntryDtos == null)
        {
            const string errorMessage = "Cannot present daily intake carbohydrate total because daily intake list is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        double carbohydrateTotal = dailyIntakeEntryDtos.Sum(dailyIntakeEntryDto => dailyIntakeEntryDto.Carbohydrates);

        return Math.Round(carbohydrateTotal, 2);
    }

    private double GetProteinTotal(List<DailyIntakeEntryDto> dailyIntakeEntryDtos)
    {
        if (dailyIntakeEntryDtos == null)
        {
            const string errorMessage = "Cannot present daily intake protein total because daily intake list is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        double proteinTotal = dailyIntakeEntryDtos.Sum(dailyIntakeEntryDto => dailyIntakeEntryDto.Protein);

        return Math.Round(proteinTotal, 2);
    }

    private double GetCalorieTotal(List<DailyIntakeEntryDto> dailyIntakeEntryDtos)
    {
        if (dailyIntakeEntryDtos == null)
        {
            const string errorMessage = "Cannot present daily intake calorie total because daily intake list is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        double calorieTotal = dailyIntakeEntryDtos.Sum(dailyIntakeEntryDto => dailyIntakeEntryDto.Calories);

        return Math.Round(calorieTotal, 2);
    }

    private double GetAverageProteinPerCalorie(List<DailyIntakeEntryDto> dailyIntakeEntryDtos)
    {
        if (dailyIntakeEntryDtos == null)
        {
            const string errorMessage = "Cannot present daily intake protein per gram because daily intake list is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (dailyIntakeEntryDtos.Count == 0)
        {
            return 0;
        }

        double proteinPerCalorieSum = dailyIntakeEntryDtos.Sum(dailyIntakeEntryDto => dailyIntakeEntryDto.ProteinPerCalorie);
        double proteinPerCalorieAverage = proteinPerCalorieSum / dailyIntakeEntryDtos.Count;

        return Math.Round(proteinPerCalorieAverage, 2);
    }
}
