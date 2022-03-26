using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net;
using System.Net.Http.Json;

namespace BlazingRecept.Client.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly string _apiAddress = "api/recipe";
        private readonly HttpClient _httpClient;

        public RecipeService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BlazingRecept.ServerAPI");
        }

        public async Task<bool> AnyAsync(string name)
        {
            try
            {
                Uri uri = new Uri(_httpClient.BaseAddress + _apiAddress + $"/{name}");
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Head, uri);
                HttpResponseMessage response = await _httpClient.SendAsync(httpRequestMessage);

                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception)
            {
            }

            return false;
        }

        public async Task<RecipeDto?> GetByIdAsync(Guid id)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_apiAddress + $"/{id}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return await response.Content.ReadFromJsonAsync<RecipeDto>();
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

        public async Task<IReadOnlyList<RecipeDto>?> GetAllAsync()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_apiAddress);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return await response.Content.ReadFromJsonAsync<IReadOnlyList<RecipeDto>>();
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        public async Task<RecipeDto?> SaveAsync(RecipeDto recipeDto)
        {
            if (recipeDto == null)
            {
                throw new ArgumentNullException(nameof(recipeDto));
            }

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_apiAddress, recipeDto);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return await response.Content.ReadFromJsonAsync<RecipeDto>();
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync(_apiAddress + $"/{id}");

                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception)
            {
            }

            return false;
        }
    }
}