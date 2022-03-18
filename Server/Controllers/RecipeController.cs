using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Mvc;

namespace BlazingRecept.Server.Controllers;

[ApiController]
[Route("api/recipes")]
public class RecipeController : ControllerBase
{
    private readonly ILogger<RecipeController> _logger;

    private readonly IRecipesService _service;

    // The Web API will only accept tokens 1) for users, and 2) having the "API.Access" scope for this API
    //static readonly string[] scopeRequiredByApi = new string[] { "API.Access" };

    public RecipeController(ILogger<RecipeController> logger, IRecipesService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    public async Task<IReadOnlyList<RecipeDto>> Get()
    {
        //HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        return await _service.GetAllAsync();
    }

    [HttpGet("{recipeIdentifier}")]
    public async Task<ActionResult<RecipeDto>> Get(string recipeIdentifier)
    {
        //HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        if (Guid.TryParse(recipeIdentifier, out Guid id))
        {
            RecipeDto recipeDto = await _service.GetByIdAsync(id);

            if (recipeDto != null)
            {
                return Ok(recipeDto);
            }
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<RecipeDto>> Post(RecipeDto recipeDto)
    {
        //HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        try
        {
            if (recipeDto == null)
            {
                return BadRequest();
            }

            return Ok(await _service.SaveAsync(recipeDto));
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        //HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        bool recipeRemoved = await _service.DeleteAsync(id);

        if (recipeRemoved)
        {
            return NoContent();
        }

        return BadRequest($"Failed to delelete entity with id: {id}");
    }
}
