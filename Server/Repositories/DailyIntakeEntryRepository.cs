using BlazingRecept.Server.Context;
using BlazingRecept.Server.Entities;
using BlazingRecept.Server.Repositories.Interfaces;

namespace BlazingRecept.Server.Repositories;

public class DailyIntakeEntryRepository : RepositoryBase<DailyIntakeEntry>, IDailyIntakeEntryRepository
{
    public DailyIntakeEntryRepository(BlazingReceptContext context) : base(context)
    {
    }
}
