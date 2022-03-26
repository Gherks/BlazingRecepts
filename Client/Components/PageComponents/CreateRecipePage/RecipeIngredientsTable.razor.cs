using BlazingRecept.Client.Pages;
using Microsoft.AspNetCore.Components;

namespace BlazingRecept.Client.Components.PageComponents.CreateRecipePage;

public partial class RecipeIngredientsTable : ComponentBase
{
    [CascadingParameter]
    public CreateRecipe? CreateRecipe { get; set; }

    private void HandleIngredientEdit(IngredientForm ingredientForm)
    {
        if (CreateRecipe == null) throw new InvalidOperationException();

        CreateRecipe.HandleAddIngredientModalOpen(ingredientForm);
    }

    private void HandleIngredientRemoval(IngredientForm ingredientForm)
    {
        if (CreateRecipe == null) throw new InvalidOperationException();

        CreateRecipe.OpenIngredientRemovalModalOpen(ingredientForm);
    }

    IReadOnlyList<IngredientForm> GetIngredientForms()
    {
        if (CreateRecipe == null) throw new InvalidOperationException();

        return CreateRecipe.IngredientForms;
    }
}
