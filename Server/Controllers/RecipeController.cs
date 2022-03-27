using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Mvc;

namespace BlazingRecept.Server.Controllers;

//[Authorize]
[ApiController]
[Route("api/recipes")]
//[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class RecipeController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    // The Web API will only accept tokens 1) for users, and 2) having the "API.Access" scope for this API
    static readonly string[] scopeRequiredByApi = new string[] { "API.Access" };

    public RecipeController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    [HttpHead("{identifier}")]
    public async Task<ActionResult> HeadAsync(string identifier)
    {
        bool ingredientExists;

        if (Guid.TryParse(identifier, out Guid id))
        {
            ingredientExists = await _recipeService.AnyAsync(id);
        }
        else
        {
            ingredientExists = await _recipeService.AnyAsync(identifier);
        }

        if (ingredientExists)
        {
            return Ok();
        }

        return NoContent();
    }

    [HttpGet]
    public async Task<IReadOnlyList<RecipeDto>> Get()
    {
        //HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        return await _recipeService.GetAllAsync();
    }

    [HttpGet("{recipeIdentifier}")]
    public async Task<ActionResult<RecipeDto>> Get(string recipeIdentifier)
    {
        //HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        if (Guid.TryParse(recipeIdentifier, out Guid id))
        {
            RecipeDto? recipeDto = await _recipeService.GetByIdAsync(id);

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

            return Ok(await _recipeService.SaveAsync(recipeDto));
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

        bool recipeRemoved = await _recipeService.DeleteAsync(id);

        if (recipeRemoved)
        {
            return NoContent();
        }

        return BadRequest($"Failed to delelete entity with id: {id}");
    }
}
