using BlazingRecept.Server.Entities.Bases;

namespace BlazingRecept.Server.Entities;

public sealed class DailyIntakeEntry : BaseEntity 
{
    public double Amount { get; set; } = -1.0;
    public int SortOrder { get; set; } = -1;
    public Guid ProductId { get; set; } = Guid.Empty;
    public Guid CollectionId { get; set; } = Guid.Empty;
}
