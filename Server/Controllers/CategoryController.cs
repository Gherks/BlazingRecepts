using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Server.Controllers;

//[Authorize]
[ApiController]
[Route("api/categories")]
//[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    // The Web API will only accept tokens 1) for users, and 2) having the "API.Access" scope for this API
    static readonly string[] scopeRequiredByApi = new string[] { "API.Access" };

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("{categoryType:int}")]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> Get(int categoryType)
    {
        //HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        if (Enum.IsDefined(typeof(CategoryType), categoryType))
        {
            return Ok(await _categoryService.GetAllOfTypeAsync((CategoryType)categoryType));
        }
        else
        {
            return BadRequest(new List<CategoryDto>());
        }
    }
}
