using Microsoft.AspNetCore.Components;
using Serilog;

namespace BlazingRecept.Client.Shared;

public partial class MainLayout
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "MainLayout";

    [Inject]
    public NavigationManager? NavigationManager { get; set; }

    private void HandleNavigationToRecipes()
    {
        if (NavigationManager == null)
        {
            string errorMessage = "Cannot handle navigation to recipes page because navigation manager has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        NavigationManager.NavigateTo("");
    }

    private void HandleNavigationToRecipeWorkbench()
    {
        if (NavigationManager == null)
        {
            string errorMessage = "Cannot handle navigation to recipe workbench page because navigation manager has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        NavigationManager.NavigateTo("recipeworkbench");
    }

    private void HandleNavigationToIngredients()
    {
        if (NavigationManager == null)
        {
            string errorMessage = "Cannot handle navigation to ingredients page because navigation manager has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        NavigationManager.NavigateTo("ingredients");
    }

    private void HandleNavigationToDailyIntake()
    {
        if (NavigationManager == null)
        {
            string errorMessage = "Cannot handle navigation to daily intake page because navigation manager has not been set.";
            Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        NavigationManager.NavigateTo("dailyintake");
    }
}
