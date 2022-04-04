using BlazingRecept.Shared.Dto.Base;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Shared.Dto;

public sealed class CategoryDto : DtoBase
{
    public string Name { get; set; } = "";
    public CategoryType CategoryType { get; set; } = CategoryType.Unassigned;
    public int SortOrder { get; set; } = 0;
}
