using System.ComponentModel.DataAnnotations;

namespace BlazingRecept.Server.Entities.Base;

public class BaseEntity
{
    [Required]
    public Guid Id { get; set; } = Guid.Empty;
}
