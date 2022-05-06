using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Serilog;
using System.Net;
using System.Net.Http.Json;

namespace BlazingRecept.Client.Services;

public class RecipeService : IRecipeService
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _apiAddress = "api/recipes";

    private readonly HttpClient _publicHttpClient;
    private readonly HttpClient _authenticatedHttpClient;

    public RecipeService(IHttpClientFactory httpClientFactory)
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
            const string errorMessage = "Failed while checking existence of recipe with name: {@Name}";
            Log.ForContext(_logProperty, GetType().Name).Error(exception, errorMessage, name);
        }

        return false;
    }

    public async Task<RecipeDto?> GetByIdAsync(Guid id)
    {
        try
        {
            HttpResponseMessage response = await _publicHttpClient.GetAsync(_apiAddress + $"/{id}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                RecipeDto? recipeDto = await response.Content.ReadFromJsonAsync<RecipeDto>();

                Contracts.LogAndThrowWhenNull(recipeDto, "Failed to read returned recipe dto after successfuly fetching it.");

                SortIngredientMeasurements(recipeDto);

                return recipeDto;
            }
        }
        catch (Exception exception)
        {
            const string errorMessage = "Failed while fetching recipe with id: {@Id}";
            Log.ForContext(_logProperty, GetType().Name).Error(exception, errorMessage, id);
        }

        return null;
    }

    public async Task<IReadOnlyList<RecipeDto>?> GetAllAsync()
    {
        try
        {
            HttpResponseMessage response = await _publicHttpClient.GetAsync(_apiAddress);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                IReadOnlyList<RecipeDto>? recipeDtos = await response.Content.ReadFromJsonAsync<IReadOnlyList<RecipeDto>>();

                Contracts.LogAndThrowWhenNull(recipeDtos, "Failed to read returned recipe dto list after successfuly fetching it.");

                foreach (RecipeDto recipeDto in recipeDtos)
                {
                    SortIngredientMeasurements(recipeDto);
                }

                return recipeDtos;
            }
        }
        catch (Exception exception)
        {
            const string errorMessage = "Failed while fetching all recipes.";
            Log.ForContext(_logProperty, GetType().Name).Error(exception, errorMessage);
        }

        return null;
    }

    public async Task<RecipeDto?> SaveAsync(RecipeDto recipeDto)
    {
        try
        {
            HttpResponseMessage response = await _authenticatedHttpClient.PostAsJsonAsync(_apiAddress, recipeDto);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                RecipeDto? savedRecipeDto = await response.Content.ReadFromJsonAsync<RecipeDto>();

                Contracts.LogAndThrowWhenNull(savedRecipeDto, "Failed to read returned recipe dto after successfuly saving it.");

                SortIngredientMeasurements(savedRecipeDto);

                return savedRecipeDto;
            }
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
        catch (Exception exception)
        {
            const string errorMessage = "Failed while saving recipe: {@RecipeDto}";
            Log.ForContext(_logProperty, GetType().Name).Error(exception, errorMessage, recipeDto);
        }

        return null;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            HttpResponseMessage response = await _authenticatedHttpClient.DeleteAsync(_apiAddress + $"/{id}");

            return response.StatusCode == HttpStatusCode.NoContent;
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
        catch (Exception exception)
        {
            const string errorMessage = "Failed while deleting recipe with id: {@Id}";
            Log.ForContext(_logProperty, GetType().Name).Error(exception, errorMessage, id);
        }

        return false;
    }

    private void SortIngredientMeasurements(RecipeDto recipeDto)
    {
        recipeDto.IngredientMeasurementDtos.Sort((first, second) => first.SortOrder > second.SortOrder ? -1 : 1);
    }
}
