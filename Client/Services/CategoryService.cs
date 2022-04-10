using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using System.Net;
using System.Net.Http.Json;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Client.Services;

public class CategoryService : ICategoryService
{
    private readonly string _apiAddress = "api/categories";
    private readonly HttpClient _publicHttpClient;

    public CategoryService(IHttpClientFactory httpClientFactory)
    {
        _publicHttpClient = httpClientFactory.CreateClient("BlazingRecept.PublicServerAPI");
    }

    public async Task<IReadOnlyList<CategoryDto>?> GetAllOfTypeAsync(CategoryType categoryType)
    {
        try
        {
            HttpResponseMessage response = await _publicHttpClient.GetAsync(_apiAddress + $"/{Convert.ToInt32(categoryType)}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadFromJsonAsync<IReadOnlyList<CategoryDto>>();
            }
        }
        catch (Exception)
        {
        }

        return null;
    }
}
