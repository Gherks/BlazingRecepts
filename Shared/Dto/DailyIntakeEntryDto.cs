using BlazingRecept.Shared.Dto.Base;

namespace BlazingRecept.Shared.Dto;

public sealed class DailyIntakeEntryDto : DtoBase
{
    public string ProductName { get; set; } = string.Empty;
    public double Amount { get; set; } = 0;
    public double Fat { get; set; } = 0;
    public double Carbohydrates { get; set; } = 0;
    public double Protein { get; set; } = 0;
    public double Calories { get; set; } = 0;
    public double ProteinPerCalorie { get; set; } = 0;
    public bool IsRecipe { get; set; } = false;
    public Guid ProductId { get; set; } = Guid.Empty;
    public Guid CollectionId { get; set; } = Guid.Empty;
}
