using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net;
using System.Net.Http.Json;

namespace BlazingRecept.Client.Services
{
    public class IngredientCategoryService : IIngredientCategoryService
    {
        private readonly string _apiAddress = "api/ingredientcategories";
        private readonly HttpClient _httpClient;

        public IngredientCategoryService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BlazingRecept.ServerAPI");
        }

        public async Task<IReadOnlyList<IngredientCategoryDto>?> GetAllAsync()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_apiAddress);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return await response.Content.ReadFromJsonAsync<IReadOnlyList<IngredientCategoryDto>>();
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
}