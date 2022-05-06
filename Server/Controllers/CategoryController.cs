using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Server.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private static readonly string _logProperty = "Domain";

    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("{identifier}")]
    public async Task<ActionResult<CategoryDto>> Get(string identifier)
    {
        if (Guid.TryParse(identifier, out Guid id))
        {
            CategoryDto? categoryDto = await _categoryService.GetByIdAsync(id);

            if (categoryDto != null)
            {
                return Ok(categoryDto);
            }
        }

        return NoContent();
    }

    [HttpGet("by-type/{categoryType:int}")]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> Get(int categoryType)
    {
        if (Enum.IsDefined(typeof(CategoryType), categoryType))
        {
            return Ok(await _categoryService.GetAllOfTypeAsync((CategoryType)categoryType));
        }
        else
        {
            const string errorMessage = "Controller couldn't fetch category because given integer({@CategoryType}) doesn't map to a category type.";
            Log.ForContext(_logProperty, GetType().Name).Error(errorMessage, categoryType);

            return BadRequest();
        }
    }
}
