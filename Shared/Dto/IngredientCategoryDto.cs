using BlazingRecept.Shared.Dto.Base;

namespace BlazingRecept.Shared.Dto;

public sealed class IngredientCategoryDto : DtoBase
{
    public string Name { get; set; } = "";
    public int SortOrder { get; set; } = 0;
}
