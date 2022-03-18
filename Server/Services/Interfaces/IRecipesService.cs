using Receptacle.Shared.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Receptacle.Server.Services.Interfaces
{
    public interface IRecipesService
    {
        Task<RecipeDto> GetByIdAsync(Guid id);
        Task<IReadOnlyList<RecipeDto>> GetAllAsync();
        Task<RecipeDto> SaveAsync(RecipeDto recipeDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
