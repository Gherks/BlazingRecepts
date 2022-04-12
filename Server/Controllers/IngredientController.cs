using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Serilog;
using Serilog.Context;

namespace BlazingRecept.Server.Controllers;

[ApiController]
[Route("api/ingredients")]
public class IngredientController : ControllerBase
{
    static readonly string[] scopeRequiredByApi = new string[] { "API.Access" };

    private readonly IIngredientService _ingredientService;

    public IngredientController(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;

        LogContext.PushProperty("Domain", "Ingredient");
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
        return await _ingredientService.GetAllAsync();
    }

    [HttpGet("sorted")]
    public async Task<IReadOnlyList<IngredientCollectionTypeDto>> GetSorted()
    {
        return await _ingredientService.GetAllSortedAsync();
    }

    [HttpGet("{ingredientIdentifier}")]
    public async Task<ActionResult<IngredientDto>> Get(string ingredientIdentifier)
    {
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

    [Authorize]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    [HttpPost]
    public async Task<ActionResult<IngredientDto>> Post(IngredientDto ingredientDto)
    {
        HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

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
            Log.Error(exception, "Controller failed while saving ingredient: {@IngredientDto}", ingredientDto);
            return BadRequest("Failed while saving ingredient.");
        }
    }

    [Authorize]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        bool ingredientRemoved = await _ingredientService.DeleteAsync(id);

        if (ingredientRemoved)
        {
            return Ok(ingredientRemoved);
        }

        Log.Error("Controller failed to delete ingredient with id: {@Id}", id);
        return BadRequest($"Failed to delete ingredient.");
    }
}
