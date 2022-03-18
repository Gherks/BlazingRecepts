using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Mvc;

namespace BlazingRecept.Server.Controllers;

[ApiController]
[Route("api/ingredients")]
public class IngredientsController : ControllerBase
{
    private readonly ILogger<IngredientsController> _logger;

    private readonly IIngredientsService _service;

    // The Web API will only accept tokens 1) for users, and 2) having the "API.Access" scope for this API
    //static readonly string[] scopeRequiredByApi = new string[] { "API.Access" };

    public IngredientsController(ILogger<IngredientsController> logger, IIngredientsService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    public async Task<IReadOnlyList<IngredientDto>> Get()
    {
        //HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        return await _service.GetAllAsync();
    }

    [HttpGet("{ingredientIdentifier}")]
    public async Task<ActionResult<IngredientDto>> Get(string ingredientIdentifier)
    {
        //HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        if (Guid.TryParse(ingredientIdentifier, out Guid id))
        {
            IngredientDto ingredientDto = await _service.GetByIdAsync(id);

            if (ingredientDto != null)
            {
                return Ok(ingredientDto);
            }
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<IngredientDto>> Post(IngredientDto ingredientDto)
    {
        //HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        try
        {
            if (ingredientDto == null)
            {
                return BadRequest();
            }

            return Ok(await _service.SaveAsync(ingredientDto));
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

        bool ingredientRemoved = await _service.DeleteAsync(id);

        if (ingredientRemoved)
        {
            return Ok(ingredientRemoved);
        }

        return BadRequest($"Failed to delelete entity with id: {id}");
    }
}
