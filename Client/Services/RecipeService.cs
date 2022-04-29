using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Serilog;
using System.Net;
using System.Net.Http.Json;

namespace BlazingRecept.Client.Services;

public class RecipeService : IRecipeService
{
    private static readonly string _logProperty = "Domain";
    private static readonly string _logDomainName = "RecipeService";
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
            const string messageTemplate = "Failed while checking existence of recipe with name: {@Name}";
            Log.ForContext(_logProperty, _logDomainName).Error(exception, messageTemplate, name);
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

                if (recipeDto == null)
                {
                    const string errorMessage = "Failed to read returned recipe after saving it.";
                    Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                SortIngredientMeasurements(recipeDto);

                return recipeDto;
            }
        }
        catch (Exception exception)
        {
            const string messageTemplate = "Failed while fetching recipe with id: {@Id}";
            Log.ForContext(_logProperty, _logDomainName).Error(exception, messageTemplate, id);
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

                if (recipeDtos == null)
                {
                    const string errorMessage = "Failed to fetch all recipe dtos.";
                    Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                foreach (RecipeDto recipeDto in recipeDtos)
                {
                    SortIngredientMeasurements(recipeDto);
                }

                return recipeDtos;
            }
        }
        catch (Exception exception)
        {
            const string messageTemplate = "Failed while fetching all recipes.";
            Log.ForContext(_logProperty, _logDomainName).Error(exception, messageTemplate);
        }

        return null;
    }

    public async Task<RecipeDto?> SaveAsync(RecipeDto recipeDto)
    {
        if (recipeDto == null)
        {
            const string messageTemplate = "Failed to save recipe because passed recipe is not set.";
            Log.ForContext(_logProperty, _logDomainName).Error(messageTemplate);
            throw new ArgumentNullException(nameof(recipeDto));
        }

        try
        {
            HttpResponseMessage response = await _authenticatedHttpClient.PostAsJsonAsync(_apiAddress, recipeDto);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                RecipeDto? savedRecipeDto = await response.Content.ReadFromJsonAsync<RecipeDto>();

                if (savedRecipeDto == null)
                {
                    const string errorMessage = "Failed to read returned recipe after saving it.";
                    Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

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
            const string messageTemplate = "Failed while saving recipe: {@RecipeDto}";
            Log.ForContext(_logProperty, _logDomainName).Error(exception, messageTemplate, recipeDto);
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
            const string messageTemplate = "Failed while deleting recipe with id: {@Id}";
            Log.ForContext(_logProperty, _logDomainName).Error(exception, messageTemplate, id);
        }

        return false;
    }

    private void SortIngredientMeasurements(RecipeDto recipeDto)
    {
        recipeDto.IngredientMeasurementDtos.Sort((first, second) => first.SortOrder > second.SortOrder ? -1 : 1);
    }
}
