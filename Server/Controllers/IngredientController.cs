using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace BlazingRecept.Server.Controllers;

//[Authorize]
[ApiController]
[Route("api/ingredient")]
//[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class IngredientController : ControllerBase
{
    private readonly IIngredientService _ingredientService;

    // The Web API will only accept tokens 1) for users, and 2) having the "API.Access" scope for this API
    static readonly string[] scopeRequiredByApi = new string[] { "API.Access" };

    public IngredientController(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    [HttpHead("{identifier}")]
    public async Task<ActionResult> HeadAsync(string identifier)
    {
        bool ingredientExists;

        if (Guid.TryParse(identifier, out Guid id))
        {
            ingredientExists = await _ingredientService.AnyAsync(id);
        }
        else
        {
            ingredientExists = await _ingredientService.AnyAsync(identifier);
        }

        if (ingredientExists)
        {
            return Ok();
        }

        return NoContent();
    }

    [HttpGet]
    public async Task<IReadOnlyList<IngredientDto>> Get()
    {
        //HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        return await _ingredientService.GetAllAsync();
    }

    [HttpGet("sorted")]
    public async Task<IReadOnlyList<IngredientCollectionTypeDto>> GetSorted()
    {
        //HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        return await _ingredientService.GetAllSortedAsync();
    }

    [HttpGet("{ingredientIdentifier}")]
    public async Task<ActionResult<IngredientDto>> Get(string ingredientIdentifier)
    {
        //HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        if (Guid.TryParse(ingredientIdentifier, out Guid id))
        {
            IngredientDto? ingredientDto = await _ingredientService.GetByIdAsync(id);

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

            return Ok(await _ingredientService.SaveAsync(ingredientDto));
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

        bool ingredientRemoved = await _ingredientService.DeleteAsync(id);

        if (ingredientRemoved)
        {
            return Ok(ingredientRemoved);
        }

        return BadRequest($"Failed to delelete entity with id: {id}");
    }
}
