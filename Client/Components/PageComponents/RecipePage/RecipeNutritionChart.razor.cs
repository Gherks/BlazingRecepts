using BlazingRecept.Client.Components.PageComponents.Base;
using BlazingRecept.Client.Pages;
using BlazingRecept.Shared;
using BlazingRecept.Shared.Extensions;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.PageComponents.RecipePage;

public partial class RecipeNutritionChart : PageComponentBase
{
    private NutritionalChartItem[]? _nutritionalChartItems = null;

    [CascadingParameter]
    protected internal Recipe? RecipePage { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        Contracts.LogAndThrowWhenNull(RecipePage, "Cannot ingredient measurement table rows because recipe page reference has not been set.");
        Contracts.LogAndThrowWhenNull(RecipePage.RecipeDto, "Cannot ingredient measurement table rows because recipe has not been set.");

        IsLoading = true;

        await base.OnInitializedAsync();

        LoadChartItems();

        IsLoading = false;
    }

    private void LoadChartItems()
    {
        Contracts.LogAndThrowWhenNull(RecipePage, "Cannot construct nutritional chart items because recipe page reference has not been set.");
        Contracts.LogAndThrowWhenNull(RecipePage.RecipeDto, "Cannot construct nutritional chart items because recipe has not been set.");

        _nutritionalChartItems = new NutritionalChartItem[] {
            new()
            {
                Label = "Fett",
                Value = RecipePage.RecipeDto.GetTotalFat()
            },
            new()
            {
                Label = "Kolhydrater",
                Value = RecipePage.RecipeDto.GetTotalCarbohydrates()
            },
            new()
            {
                Label = "Protein",
                Value = RecipePage.RecipeDto.GetTotalProtein()
            }
        };
    }

    private class NutritionalChartItem
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; } = 0.0;
    }
}
