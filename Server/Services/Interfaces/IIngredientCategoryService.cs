using Receptacle.Shared.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Receptacle.Server.Services.Interfaces
{
    public interface IIngredientCategoryService
    {
        Task<IReadOnlyList<IngredientCategoryDto>> GetAllAsync();
    }
}
