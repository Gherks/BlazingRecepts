using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Serilog;
using Serilog.Context;

namespace BlazingRecept.Server.Controllers;

[ApiController]
[Route("api/recipes")]
public class RecipeController : ControllerBase
{
    static readonly string[] scopeRequiredByApi = new string[] { "API.Access" };

    private readonly IRecipeService _recipeService;

    public RecipeController(IRecipeService recipeService)
    {
        _recipeService = recipeService;

        LogContext.PushProperty("Domain", "Recipe");
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
        return await _recipeService.GetAllAsync();
    }

    [HttpGet("{recipeIdentifier}")]
    public async Task<ActionResult<RecipeDto>> Get(string recipeIdentifier)
    {
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

    [Authorize]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    [HttpPost]
    public async Task<ActionResult<RecipeDto>> Post(RecipeDto recipeDto)
    {
        HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

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
            Log.Error(exception, "Controller failed while saving recipe: {@RecipeDto}", recipeDto);
            return BadRequest("Failed while saving recipe.");
        }
    }

    [Authorize]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        bool recipeRemoved = await _recipeService.DeleteAsync(id);

        if (recipeRemoved)
        {
            return NoContent();
        }

        Log.Error("Controller failed to delete recipe with id: {@Id}", id);
        return BadRequest($"Failed to delete recipe.");
    }
}
