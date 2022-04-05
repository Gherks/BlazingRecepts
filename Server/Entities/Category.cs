using BlazingRecept.Server.Entities.Bases;
using System.ComponentModel.DataAnnotations.Schema;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Server.Entities;

public sealed class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public CategoryType CategoryType { get; set; } = CategoryType.Unassigned;
    public int SortOrder { get; set; } = -1;
}
