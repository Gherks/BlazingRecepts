using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Client.Services.Interfaces;

public interface IDailyIntakeEntryService
{
    Task<bool> AnyAsync(string name);
    Task<DailyIntakeEntryDto?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<DailyIntakeEntryDto>?> GetAllAsync();
    Task<DailyIntakeEntryDto?> SaveAsync(DailyIntakeEntryDto dailyIntakeEntryDto);
    Task<bool> SaveAsync(List<DailyIntakeEntryDto> dailyIntakeEntryDtos);
    Task<bool> DeleteAsync(Guid id);
}
