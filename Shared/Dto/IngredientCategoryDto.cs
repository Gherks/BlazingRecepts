using Receptacle.Shared.Dto.Base;

namespace Receptacle.Shared.Dto;

public class IngredientCategoryDto : DtoBase
{
    public string Name { get; set; } = "";
    public int SortOrder { get; set; } = 0;
}
