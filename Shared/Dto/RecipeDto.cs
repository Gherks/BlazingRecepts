using BlazingRecept.Shared.Dto.Base;

namespace BlazingRecept.Shared.Dto;

public sealed class RecipeDto : DtoBase
{
    public string Name { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public int PortionAmount { get; set; } = -1;
    public Guid CategoryId { get; set; } = Guid.Empty;
    public CategoryDto CategoryDto { get; set; } = new();
    public List<IngredientMeasurementDto> IngredientMeasurementDtos { get; set; } = new();
}
