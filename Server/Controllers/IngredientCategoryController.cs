using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Receptacle.Server.Services.Interfaces;
using Receptacle.Shared.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Receptacle.Server.Controllers;

[ApiController]
[Route("api/ingredientcategories")]
public class IngredientCategoryController : ControllerBase
{
    private readonly ILogger<IngredientCategoryController> _logger;

    private readonly IIngredientCategoryService _service;

    // The Web API will only accept tokens 1) for users, and 2) having the "API.Access" scope for this API
    //static readonly string[] scopeRequiredByApi = new string[] { "API.Access" };

    public IngredientCategoryController(ILogger<IngredientCategoryController> logger, IIngredientCategoryService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    public async Task<IReadOnlyList<IngredientCategoryDto>> Get()
    {
        //HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        return await _service.GetAllAsync();
    }
}
