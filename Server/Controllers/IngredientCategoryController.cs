using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace BlazingRecept.Server.Controllers;

//[Authorize]
[ApiController]
[Route("api/ingredientcategory")]
//[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class IngredientCategoryController : ControllerBase
{
    private readonly IIngredientCategoryService _service;

    // The Web API will only accept tokens 1) for users, and 2) having the "API.Access" scope for this API
    static readonly string[] scopeRequiredByApi = new string[] { "API.Access" };

    public IngredientCategoryController(IIngredientCategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IReadOnlyList<IngredientCategoryDto>> Get()
    {
        //HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        return await _service.GetAllAsync();
    }
}
