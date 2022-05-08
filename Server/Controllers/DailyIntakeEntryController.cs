using BlazingRecept.Server.Services.Interfaces;
using BlazingRecept.Shared;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace BlazingRecept.Server.Controllers;

[ApiController]
[Route("api/daily-intake-entries")]
public class DailyIntakeEntryController : ControllerBase
{
    private static readonly string[] _scopeRequiredByApi = new string[] { "API.Access" };

    private readonly IDailyIntakeEntryService _dailyIntakeEntryService;

    public DailyIntakeEntryController(IDailyIntakeEntryService dailyIntakeEntryService)
    {
        _dailyIntakeEntryService = dailyIntakeEntryService;
    }

    [HttpHead("{identifier}")]
    public async Task<ActionResult> HeadAsync(string identifier)
    {
        bool dailyIntakeEntryExists;

        if (Guid.TryParse(identifier, out Guid id))
        {
            dailyIntakeEntryExists = await _dailyIntakeEntryService.AnyAsync(id);
        }
        else
        {
            return BadRequest();
        }

        if (dailyIntakeEntryExists)
        {
            return Ok();
        }

        return NoContent();
    }

    [HttpGet]
    public async Task<IReadOnlyList<DailyIntakeEntryDto>?> Get()
    {
        return await _dailyIntakeEntryService.GetAllAsync();
    }

    [HttpGet("{identifier}")]
    public async Task<ActionResult<DailyIntakeEntryDto>> Get(string identifier)
    {
        if (Guid.TryParse(identifier, out Guid id))
        {
            DailyIntakeEntryDto? dailyIntakeEntryDto = await _dailyIntakeEntryService.GetByIdAsync(id);

            if (dailyIntakeEntryDto != null)
            {
                return Ok(dailyIntakeEntryDto);
            }
        }

        return NoContent();
    }

    [Authorize]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    [HttpPost]
    public async Task<ActionResult<DailyIntakeEntryDto>> Post(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        HttpContext.VerifyUserHasAnyAcceptedScope(_scopeRequiredByApi);

        try
        {
            if (dailyIntakeEntryDto == null)
            {
                return BadRequest();
            }

            return Ok(await _dailyIntakeEntryService.SaveAsync(dailyIntakeEntryDto));
        }
        catch (Exception exception)
        {
            Log.Error(exception, $"Controller failed while saving daily intake entry: {dailyIntakeEntryDto}");
            return BadRequest();
        }
    }

    [Authorize]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    [HttpPost("many")]
    public async Task<ActionResult<bool>> Post(List<DailyIntakeEntryDto> dailyIntakeEntryDtos)
    {
        HttpContext.VerifyUserHasAnyAcceptedScope(_scopeRequiredByApi);

        try
        {
            if (dailyIntakeEntryDtos == null)
            {
                return BadRequest();
            }

            bool saveSuccessful = await _dailyIntakeEntryService.SaveAsync(dailyIntakeEntryDtos);

            if (saveSuccessful)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, $"Controller failed while saving daily intake entries: {dailyIntakeEntryDtos}");
            return BadRequest();
        }
    }

    [Authorize]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        HttpContext.VerifyUserHasAnyAcceptedScope(_scopeRequiredByApi);

        bool dailyIntakeEntryRemoved = await _dailyIntakeEntryService.DeleteAsync(id);

        if (dailyIntakeEntryRemoved)
        {
            return Ok(dailyIntakeEntryRemoved);
        }

        Log.Error($"Controller failed to delete daily intake entry with id: {id}");
        return BadRequest($"Failed to delete daily intake entry.");
    }
}
