using BlazingRecept.Client.Extensions;
using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Serilog;
using System.Net;
using System.Net.Http.Json;

namespace BlazingRecept.Client.Services;

public class DailyIntakeEntryService : IDailyIntakeEntryService
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "DailyIntakeService";
    private static readonly string _apiAddress = "api/daily-intake-entries";

    private readonly HttpClient _publicHttpClient;
    private readonly HttpClient _authenticatedHttpClient;

    public DailyIntakeEntryService(IHttpClientFactory httpClientFactory)
    {
        _publicHttpClient = httpClientFactory.CreateClient("BlazingRecept.PublicServerAPI");
        _authenticatedHttpClient = httpClientFactory.CreateClient("BlazingRecept.AuthenticatedServerAPI");
    }

    public async Task<bool> AnyAsync(string name)
    {
        try
        {
            Uri uri = new Uri(_publicHttpClient.BaseAddress + _apiAddress + $"/{name}");
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Head, uri);
            HttpResponseMessage response = await _publicHttpClient.SendAsync(httpRequestMessage);

            return response.StatusCode == HttpStatusCode.OK;
        }
        catch (Exception exception)
        {
            const string messageTemplate = "Failed while checking existence of daily intake entry with name: {@Name}";
            Log.ForContext(_logProperty, _logDomainName).Error(exception, messageTemplate, name);
        }

        return false;
    }

    public async Task<DailyIntakeEntryDto?> GetByIdAsync(Guid id)
    {
        try
        {
            HttpResponseMessage response = await _publicHttpClient.GetAsync(_apiAddress + $"/{id}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadFromJsonAsync<DailyIntakeEntryDto>();
            }
        }
        catch (Exception exception)
        {
            const string messageTemplate = "Failed while fetching daily intake entry with id: {@Id}";
            Log.ForContext(_logProperty, _logDomainName).Error(exception, messageTemplate, id);
        }

        return null;
    }

    public async Task<IReadOnlyList<DailyIntakeEntryDto>?> GetAllAsync()
    {
        try
        {
            HttpResponseMessage response = await _publicHttpClient.GetAsync(_apiAddress);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadFromJsonAsync<IReadOnlyList<DailyIntakeEntryDto>>();
            }
        }
        catch (Exception exception)
        {
            const string messageTemplate = "Failed while fetching all daily intake collections.";
            Log.ForContext(_logProperty, _logDomainName).Error(exception, messageTemplate);
        }

        return null;
    }

    public async Task<DailyIntakeEntryDto?> SaveAsync(DailyIntakeEntryDto dailyIntakeEntryDto)
    {
        try
        {
            HttpResponseMessage response = await _authenticatedHttpClient.PostAsJsonAsync(_apiAddress, dailyIntakeEntryDto);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadFromJsonAsync<DailyIntakeEntryDto>();
            }
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
        catch (Exception exception)
        {
            const string messageTemplate = "Failed while saving daily intake entry: {@DailyIntakeEntryDto}";
            Log.ForContext(_logProperty, _logDomainName).Error(exception, messageTemplate, dailyIntakeEntryDto);
        }

        return null;
    }

    public async Task<bool> SaveAsync(List<DailyIntakeEntryDto> dailyIntakeEntryDtos)
    {
        try
        {
            HttpResponseMessage response = await _authenticatedHttpClient.PostAsJsonAsync(_apiAddress + "/many", dailyIntakeEntryDtos);

            return response.StatusCode == HttpStatusCode.OK;
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
        catch (Exception exception)
        {
            const string messageTemplate = "Failed while saving daily intake entries: {@DailyIntakeEntryDtos}";
            Log.ForContext(_logProperty, _logDomainName).Error(exception, messageTemplate, dailyIntakeEntryDtos);
        }

        return false;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            HttpResponseMessage response = await _authenticatedHttpClient.DeleteAsync(_apiAddress + $"/{id}");

            return response.StatusCode == HttpStatusCode.OK;
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
        catch (Exception exception)
        {
            const string messageTemplate = "Failed while deleting daily intake entry with id: {@Id}";
            Log.ForContext(_logProperty, _logDomainName).Error(exception, messageTemplate, id);
        }

        return false;
    }
}
