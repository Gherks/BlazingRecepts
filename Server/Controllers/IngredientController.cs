using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Serilog;

namespace BlazingRecept.Server.Controllers;

[ApiController]
[Route("api/ingredients")]
public class IngredientController : ControllerBase
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "IngredientController";
    private static readonly string[] _scopeRequiredByApi = new string[] { "API.Access" };

    private readonly IIngredientService _ingredientService;

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
    public async Task<IReadOnlyList<IngredientDto>?> Get()
    {
        return await _ingredientService.GetAllAsync();
    }

    [HttpGet("{identifier}")]
    public async Task<ActionResult<IngredientDto>> Get(string identifier)
    {
        if (Guid.TryParse(identifier, out Guid id))
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
        HttpContext.VerifyUserHasAnyAcceptedScope(_scopeRequiredByApi);

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
            const string errorMessage = "Controller failed while saving ingredient: {@IngredientDto}";
            Log.ForContext(_logProperty, _logDomainName).Error(exception, errorMessage, ingredientDto);

            return BadRequest();
        }
    }

    [Authorize]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        HttpContext.VerifyUserHasAnyAcceptedScope(_scopeRequiredByApi);

        bool ingredientRemoved = await _ingredientService.DeleteAsync(id);

        if (ingredientRemoved)
        {
            return Ok(ingredientRemoved);
        }

        const string errorMessage = "Controller failed to delete ingredient with id: {@Id}";
        Log.ForContext(_logProperty, _logDomainName).Error(errorMessage, id);

        return BadRequest();
    }
}
