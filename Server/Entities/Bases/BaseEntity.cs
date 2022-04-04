using System.ComponentModel.DataAnnotations;

namespace BlazingRecept.Server.Entities.Bases;

public class BaseEntity
{
    [Required]
    public Guid Id { get; set; } = Guid.Empty;
}
