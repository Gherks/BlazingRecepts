using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace BlazingRecept.Client.Components.PageComponents.DailyIntakePage;

public partial class DailyIntakeTable : PageComponentBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "DailyIntakeTable";

    private List<CheckableDailyIntakeEntry> _checkableDailyIntakeEntries = new();
    private DailyIntakeEntryDto? _uneditedDailyIntakeEntryDto = null;

    [Parameter]
    public Guid CollectionId { get; set; }

    [Parameter]
    public List<DailyIntakeEntryDto>? DailyIntakeEntryDtos { get; set; }

    [Parameter]
    public Func<DailyIntakeTable, Guid, Task<bool>>? OnDailyIntakeEntryAddAsync { get; set; }

    [Parameter]
    public Func<DailyIntakeTable, DailyIntakeEntryDto, Task<bool>>? OnDailyIntakeEntryMoveUpInOrderAsync { get; set; }

    [Parameter]
    public Func<DailyIntakeTable, DailyIntakeEntryDto, Task<bool>>? OnDailyIntakeEntryMoveDownInOrderAsync { get; set; }

    [Parameter]
    public Func<DailyIntakeEntryDto, Task<bool>>? OnDailyIntakeEntryEditSubmitAsync { get; set; }

    [Parameter]
    public Func<DailyIntakeTable, DailyIntakeEntryDto, Task<bool>>? OnDailyIntakeEntryRemoveAsync { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (DailyIntakeEntryDtos == null)
        {
            const string errorMessage = "Cannot load daily intake entry table because daily intake entry dto list is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        ConstructCheckableDailyIntakeEntryList(DailyIntakeEntryDtos);
    }

    public void ConstructCheckableDailyIntakeEntryList(List<DailyIntakeEntryDto> DailyIntakeEntryDtos)
    {
        _checkableDailyIntakeEntries.Clear();

        foreach (DailyIntakeEntryDto dailyIntakeEntryDto in DailyIntakeEntryDtos)
        {
            _checkableDailyIntakeEntries.Add(new()
            {
                IsChecked = false,
                DailyIntakeEntryDto = dailyIntakeEntryDto
            });
        }

        StateHasChanged();
    }

    private async Task HandleDailyIntakeEntryMoveUpInOrderClickAsync(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        if (OnDailyIntakeEntryMoveUpInOrderAsync != null)
        {
            await OnDailyIntakeEntryMoveUpInOrderAsync.Invoke(this, dailyIntakeEntryDto);
        }
    }

    private async Task HandleDailyIntakeEntryMoveDownInOrderClickAsync(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        if (OnDailyIntakeEntryMoveDownInOrderAsync != null)
        {
            await OnDailyIntakeEntryMoveDownInOrderAsync.Invoke(this, dailyIntakeEntryDto);
        }
    }

    private async Task HandleDailyIntakeEntryEditSubmitClickAsync(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        if (OnDailyIntakeEntryEditSubmitAsync != null)
        {
            bool successfullyEdited = await OnDailyIntakeEntryEditSubmitAsync.Invoke(dailyIntakeEntryDto);

            if (successfullyEdited)
            {
                _uneditedDailyIntakeEntryDto = null;
            }
        }
    }

    private async Task HandleDailyIntakeEntryRemoveClickAsync(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        if (OnDailyIntakeEntryRemoveAsync != null)
        {
            await OnDailyIntakeEntryRemoveAsync.Invoke(this, dailyIntakeEntryDto);
        }
    }

    private async Task HandleDailyIntakeEntryAddClickAsync()
    {
        if (OnDailyIntakeEntryAddAsync != null)
        {
            await OnDailyIntakeEntryAddAsync.Invoke(this, CollectionId);
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

    private string GetDailyIntakeEntryRowClass(CheckableDailyIntakeEntry checkableDailyIntakeEntry)
    {
        if (checkableDailyIntakeEntry == null)
        {
            const string errorMessage = "Cannot set class on daily intake entry table row because checkable daily intake entry has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        return checkableDailyIntakeEntry.IsChecked ? "table-primary" : "";
    }
    private bool AnyRowHasBeenChecked()
    {
        return _checkableDailyIntakeEntries.Any(checkableDailyIntakeEntry => checkableDailyIntakeEntry.IsChecked == true);
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

    private double GetFatTotal()
    {
        if (DailyIntakeEntryDtos == null)
        {
            const string errorMessage = "Cannot present daily intake grams total because daily intake list is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        double fatTotal = DailyIntakeEntryDtos.Sum(dailyIntakeEntryDto => dailyIntakeEntryDto.Fat);

        return Math.Round(fatTotal, 2);
    }

    private double GetCarbohydrateTotal()
    {
        if (DailyIntakeEntryDtos == null)
        {
            const string errorMessage = "Cannot present daily intake carbohydrate total because daily intake list is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        double carbohydrateTotal = DailyIntakeEntryDtos.Sum(dailyIntakeEntryDto => dailyIntakeEntryDto.Carbohydrates);

        return Math.Round(carbohydrateTotal, 2);
    }

    private double GetProteinTotal()
    {
        if (DailyIntakeEntryDtos == null)
        {
            const string errorMessage = "Cannot present daily intake protein total because daily intake list is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        double proteinTotal = DailyIntakeEntryDtos.Sum(dailyIntakeEntryDto => dailyIntakeEntryDto.Protein);

        return Math.Round(proteinTotal, 2);
    }

    private double GetCalorieTotal()
    {
        if (DailyIntakeEntryDtos == null)
        {
            const string errorMessage = "Cannot present daily intake calorie total because daily intake list is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        double calorieTotal = DailyIntakeEntryDtos.Sum(dailyIntakeEntryDto => dailyIntakeEntryDto.Calories);

        return Math.Round(calorieTotal, 2);
    }

    private double GetAverageProteinPerCalorie()
    {
        if (DailyIntakeEntryDtos == null)
        {
            const string errorMessage = "Cannot present daily intake protein per gram because daily intake list is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        if (DailyIntakeEntryDtos.Count == 0)
        {
            return 0;
        }

        double proteinPerCalorieSum = DailyIntakeEntryDtos.Sum(dailyIntakeEntryDto => dailyIntakeEntryDto.ProteinPerCalorie);
        double proteinPerCalorieAverage = proteinPerCalorieSum / DailyIntakeEntryDtos.Count;

        return Math.Round(proteinPerCalorieAverage, 2);
    }

    private double GetCheckedFatTotal()
    {
        double checkedFatTotal = 0.0;

        foreach (CheckableDailyIntakeEntry checkableDailyIntakeEntry in _checkableDailyIntakeEntries)
        {
            if (checkableDailyIntakeEntry.IsChecked)
            {
                checkedFatTotal += checkableDailyIntakeEntry.DailyIntakeEntryDto.Fat;
            }
        }

        return Math.Round(checkedFatTotal, 2);
    }

    private double GetCheckedCarbohydrateTotal()
    {
        double checkedCarbohydrateTotal = 0.0;

        foreach (CheckableDailyIntakeEntry checkableDailyIntakeEntry in _checkableDailyIntakeEntries)
        {
            if (checkableDailyIntakeEntry.IsChecked)
            {
                checkedCarbohydrateTotal += checkableDailyIntakeEntry.DailyIntakeEntryDto.Carbohydrates;
            }
        }

        return Math.Round(checkedCarbohydrateTotal, 2);
    }

    private double GetCheckedProteinTotal()
    {
        double checkedProteinTotal = 0.0;

        foreach (CheckableDailyIntakeEntry checkableDailyIntakeEntry in _checkableDailyIntakeEntries)
        {
            if (checkableDailyIntakeEntry.IsChecked)
            {
                checkedProteinTotal += checkableDailyIntakeEntry.DailyIntakeEntryDto.Protein;
            }
        }

        return Math.Round(checkedProteinTotal, 2);
    }

    private double GetCheckedCalorieTotal()
    {
        double checkedCalorieTotal = 0.0;

        foreach (CheckableDailyIntakeEntry checkableDailyIntakeEntry in _checkableDailyIntakeEntries)
        {
            if (checkableDailyIntakeEntry.IsChecked)
            {
                checkedCalorieTotal += checkableDailyIntakeEntry.DailyIntakeEntryDto.Calories;
            }
        }

        return Math.Round(checkedCalorieTotal, 2);
    }

    private double GetCheckedAverageProteinPerCalorie()
    {
        double checkedProteinPerCalorie = 0.0;
        int checkedAmount = 0;

        foreach (CheckableDailyIntakeEntry checkableDailyIntakeEntry in _checkableDailyIntakeEntries)
        {
            if (checkableDailyIntakeEntry.IsChecked)
            {
                checkedProteinPerCalorie += checkableDailyIntakeEntry.DailyIntakeEntryDto.Fat;
                checkedAmount++;
            }
        }

        double checkedAverageProteinPerCalorie = checkedProteinPerCalorie / checkedAmount;
        return Math.Round(checkedAverageProteinPerCalorie, 2);
    }

    private class CheckableDailyIntakeEntry
    {
        public bool IsChecked { get; set; } = false;
        public DailyIntakeEntryDto DailyIntakeEntryDto { get; set; } = new();
    }
}
