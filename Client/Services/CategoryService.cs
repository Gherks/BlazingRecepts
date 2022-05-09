using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Logging;
using BlazingRecept.Shared.Dto;
using System.Net;
using System.Net.Http.Json;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Client.Services;

public class CategoryService : ICategoryService
{
    private static readonly string _apiAddress = "api/categories";

    private readonly HttpClient _publicHttpClient;

    public CategoryService(IHttpClientFactory httpClientFactory)
    {
        _publicHttpClient = httpClientFactory.CreateClient("BlazingRecept.PublicServerAPI");
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id)
    {
        try
        {
            HttpResponseMessage response = await _publicHttpClient.GetAsync(_apiAddress + $"/{id}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadFromJsonAsync<CategoryDto>();
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, $"Failed while fetching category with id: {id}");
        }

        return null;
    }

    public async Task<IReadOnlyList<CategoryDto>?> GetAllOfTypeAsync(CategoryType categoryType)
    {
        try
        {
            HttpResponseMessage response = await _publicHttpClient.GetAsync(_apiAddress + $"/by-type/{Convert.ToInt32(categoryType)}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadFromJsonAsync<IReadOnlyList<CategoryDto>>();
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, $"Failed while fetching categories of certain type: {categoryType}");
        }

        return null;
    }
}
