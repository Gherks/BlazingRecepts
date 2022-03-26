using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net;
using System.Net.Http.Json;

namespace BlazingRecept.Client.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly string _apiAddress = "api/ingredient";
        private readonly HttpClient _httpClient;

        public IngredientService(IHttpClientFactory httpClientFactory)
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

        public async Task<IngredientDto?> GetByIdAsync(Guid id)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_apiAddress + $"/{id}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return await response.Content.ReadFromJsonAsync<IngredientDto>();
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

        public async Task<IReadOnlyList<IngredientDto>?> GetAllAsync()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_apiAddress);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return await response.Content.ReadFromJsonAsync<IReadOnlyList<IngredientDto>>();
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

        public async Task<IReadOnlyList<IngredientCollectionTypeDto>?> GetAllSortedAsync()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_apiAddress + "/sorted");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return await response.Content.ReadFromJsonAsync<IReadOnlyList<IngredientCollectionTypeDto>>();
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

        public async Task<IngredientDto?> SaveAsync(IngredientDto ingredientDto)
        {
            if (ingredientDto == null)
            {
                throw new ArgumentNullException(nameof(ingredientDto));
            }

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_apiAddress, ingredientDto);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return await response.Content.ReadFromJsonAsync<IngredientDto>();
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

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync(_apiAddress + $"/{id}");

                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
            catch (Exception)
            {
            }

            return false;
        }
    }
}