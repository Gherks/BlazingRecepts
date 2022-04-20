using AutoMapper;
using BlazingRecept.Server.Entities;
using BlazingRecept.Shared.Dto;

namespace BlazingRecept.Server.AutoMapperProfiles;

public class DailyIntakeEntryProfile : Profile
{
    public DailyIntakeEntryProfile()
    {
        CreateMap<DailyIntakeEntry, DailyIntakeEntryDto>().ReverseMap();
    }
}
