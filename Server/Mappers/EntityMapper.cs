using BlazingRecept.Server.Entities;
using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.Mappers;

public static class EntityMapper
{
    public static CategoryDto ToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            CategoryType = category.CategoryType,
            SortOrder = category.SortOrder
        };
    }

    public static Category ToEntity(CategoryDto dto)
    {
        return new Category
        {
            Id = dto.Id,
            Name = dto.Name,
            CategoryType = dto.CategoryType,
            SortOrder = dto.SortOrder
        };
    }

    public static IngredientDto ToDto(Ingredient ingredient)
    {
        return new IngredientDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Fat = ingredient.Fat,
            Carbohydrates = ingredient.Carbohydrates,
            Protein = ingredient.Protein,
            Calories = ingredient.Calories,
            CategoryDto = ToDto(ingredient.Category)
        };
    }

    public static Ingredient ToEntity(IngredientDto dto)
    {
        return new Ingredient
        {
            Id = dto.Id,
            Name = dto.Name,
            Fat = dto.Fat,
            Carbohydrates = dto.Carbohydrates,
            Protein = dto.Protein,
            Calories = dto.Calories,
            CategoryId = dto.CategoryDto.Id,
            Category = ToEntity(dto.CategoryDto)
        };
    }

    public static IngredientMeasurementDto ToDto(IngredientMeasurement measurement)
    {
        return new IngredientMeasurementDto
        {
            Id = measurement.Id,
            Measurement = measurement.Measurement,
            MeasurementUnit = measurement.MeasurementUnit,
            Grams = measurement.Grams,
            Note = measurement.Note,
            SortOrder = measurement.SortOrder,
            IngredientDto = ToDto(measurement.Ingredient)
        };
    }

    public static IngredientMeasurement ToEntity(IngredientMeasurementDto dto)
    {
        return new IngredientMeasurement
        {
            Id = dto.Id,
            Measurement = dto.Measurement,
            MeasurementUnit = dto.MeasurementUnit,
            Grams = dto.Grams,
            Note = dto.Note,
            SortOrder = dto.SortOrder,
            IngredientId = dto.IngredientDto.Id,
            Ingredient = ToEntity(dto.IngredientDto)
        };
    }

    public static RecipeDto ToDto(Recipe recipe)
    {
        return new RecipeDto
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Instructions = recipe.Instructions,
            PortionAmount = recipe.PortionAmount,
            CategoryDto = ToDto(recipe.Category),
            IngredientMeasurementDtos = recipe.IngredientMeasurements.Select(ToDto).ToList()
        };
    }

    public static Recipe ToEntity(RecipeDto dto)
    {
        return new Recipe
        {
            Id = dto.Id,
            Name = dto.Name,
            Instructions = dto.Instructions,
            PortionAmount = dto.PortionAmount,
            CategoryId = dto.CategoryDto.Id,
            Category = ToEntity(dto.CategoryDto),
            IngredientMeasurements = dto.IngredientMeasurementDtos.Select(ToEntity).ToList()
        };
    }

    public static DailyIntakeEntryDto ToDto(DailyIntakeEntry entry)
    {
        return new DailyIntakeEntryDto
        {
            Id = entry.Id,
            Amount = entry.Amount,
            SortOrder = entry.SortOrder,
            ProductId = entry.ProductId,
            CollectionId = entry.CollectionId
        };
    }

    public static DailyIntakeEntry ToEntity(DailyIntakeEntryDto dto)
    {
        return new DailyIntakeEntry
        {
            Id = dto.Id,
            Amount = dto.Amount,
            SortOrder = dto.SortOrder,
            ProductId = dto.ProductId,
            CollectionId = dto.CollectionId
        };
    }
}
