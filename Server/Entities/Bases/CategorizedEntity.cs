using System.ComponentModel.DataAnnotations;

namespace BlazingRecept.Server.Entities.Bases;

public class CategorizedEntity : BaseEntity
{
    public Guid CategoryId { get; set; } = Guid.Empty;
    public Category Category { get; set; } = new();
}
