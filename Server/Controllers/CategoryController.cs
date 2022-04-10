using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Server.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("{categoryType:int}")]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> Get(int categoryType)
    {
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
