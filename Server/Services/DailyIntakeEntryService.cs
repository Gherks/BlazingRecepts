using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;
using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Serilog;

namespace BlazingRecept.Server.Services
{
    public class DailyIntakeEntryService : IDailyIntakeEntryService
    {
        private readonly IDailyIntakeEntryRepository _dailyIntakeEntryRepository;
        private readonly IMapper _mapper;

        public DailyIntakeEntryService(IDailyIntakeEntryRepository dailyIntakeEntryRepository, IMapper mapper)
        {
            _dailyIntakeEntryRepository = dailyIntakeEntryRepository;
            _mapper = mapper;
        }

        public async Task<bool> AnyAsync(Guid id)
        {
            return await _dailyIntakeEntryRepository.AnyAsync(id);
        }

        public async Task<DailyIntakeEntryDto?> GetByIdAsync(Guid id)
        {
            DailyIntakeEntry? dailyIntakeEntry = await _dailyIntakeEntryRepository.GetByIdAsync(id);

            if (dailyIntakeEntry != null)
            {
                return _mapper.Map<DailyIntakeEntryDto>(dailyIntakeEntry);
            }

            return null;
        }

        public async Task<IReadOnlyList<DailyIntakeEntryDto>?> GetAllAsync()
        {
            IReadOnlyList<DailyIntakeEntry>? dailyIntakeEntries = await _dailyIntakeEntryRepository.ListAllAsync();

            if (dailyIntakeEntries != null)
            {
                return dailyIntakeEntries.Select(dailyIntakeEntry => _mapper.Map<DailyIntakeEntryDto>(dailyIntakeEntry)).ToList();
            }

            return null;
        }

        public async Task<DailyIntakeEntryDto> SaveAsync(DailyIntakeEntryDto dailyIntakeEntryDto)
        {
            DailyIntakeEntry dailyIntakeEntry = _mapper.Map<DailyIntakeEntry>(dailyIntakeEntryDto);

            if (dailyIntakeEntry.Id == Guid.Empty)
            {
                dailyIntakeEntry = await _dailyIntakeEntryRepository.AddAsync(dailyIntakeEntry);
            }
            else
            {
                dailyIntakeEntry = await _dailyIntakeEntryRepository.UpdateAsync(dailyIntakeEntry);
            }

            return _mapper.Map<DailyIntakeEntryDto>(dailyIntakeEntry);
        }

        public async Task<bool> SaveAsync(List<DailyIntakeEntryDto> dailyIntakeEntryDtos)
        {
            foreach (DailyIntakeEntryDto dailyIntakeEntryDto in dailyIntakeEntryDtos)
            {
                DailyIntakeEntry dailyIntakeEntry = _mapper.Map<DailyIntakeEntry>(dailyIntakeEntryDto);

                try
                {
                    if (dailyIntakeEntry.Id == Guid.Empty)
                    {
                        await _dailyIntakeEntryRepository.AddAsync(dailyIntakeEntry);
                    }
                    else
                    {
                        await _dailyIntakeEntryRepository.UpdateAsync(dailyIntakeEntry);
                    }
                }
                catch (Exception exception)
                {
                    const string errorMessage = "Something when wrong when saving and/or updating multiple daily intake entries: ({@DailyIntakeEntries})";
                    Log.Error(exception, errorMessage, dailyIntakeEntryDtos);
                }
            }

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            DailyIntakeEntry? dailyIntakeEntry = await _dailyIntakeEntryRepository.GetByIdAsync(id);

            if (dailyIntakeEntry != null)
            {
                return await _dailyIntakeEntryRepository.DeleteAsync(dailyIntakeEntry);
            }

            return false;
        }
    }
}
