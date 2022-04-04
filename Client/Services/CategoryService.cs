using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net;
using System.Net.Http.Json;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Client.Services;

public class CategoryService : ICategoryService
{
    private readonly string _apiAddress = "api/categories";
    private readonly HttpClient _httpClient;

    public CategoryService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("BlazingRecept.ServerAPI");
    }

    public async Task<IReadOnlyList<CategoryDto>?> GetAllOfTypeAsync(CategoryType categoryType)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_apiAddress + $"/{Convert.ToInt32(categoryType)}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadFromJsonAsync<IReadOnlyList<CategoryDto>>();
            }
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
        catch (Exception)
        {
        }

        return null;
    }
}
