using BlazingRecept.Server.Entities.Bases;
using System.ComponentModel.DataAnnotations.Schema;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Server.Entities;

[Table("Category")]
public sealed class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public CategoryType Type { get; set; } = CategoryType.Unassigned;
    public int SortOrder { get; set; } = -1;
    public List<CategorizedEntity> CategorizedEntities { get; set; } = new();
}
