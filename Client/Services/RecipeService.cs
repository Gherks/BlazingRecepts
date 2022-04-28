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

    private readonly ICategoryService _categoryService;

    public RecipeService(IHttpClientFactory httpClientFactory, ICategoryService categoryService)
    {
        _publicHttpClient = httpClientFactory.CreateClient("BlazingRecept.PublicServerAPI");
        _authenticatedHttpClient = httpClientFactory.CreateClient("BlazingRecept.AuthenticatedServerAPI");

        _categoryService = categoryService;
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
                RecipeDto? returnedRecipeDto = await response.Content.ReadFromJsonAsync<RecipeDto>();

                if (returnedRecipeDto == null)
                {
                    const string errorMessage = "Failed to read returned recipe after saving it.";
                    Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                returnedRecipeDto = await LoadRecipeDtoFromRecipe(returnedRecipeDto);

                return returnedRecipeDto;
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
                IReadOnlyList<RecipeDto>? readonlyRecipeDtos = await response.Content.ReadFromJsonAsync<IReadOnlyList<RecipeDto>>();

                if (readonlyRecipeDtos == null)
                {
                    const string errorMessage = "Failed to fetch all recipe dtos.";
                    Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                List<RecipeDto>? recipeDtos = readonlyRecipeDtos.ToList();

                if (recipeDtos == null)
                {
                    const string errorMessage = "Failed to fetch all recipe dtos.";
                    Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                for(int index = 0; index < recipeDtos.Count; index++)
                {
                    recipeDtos[index] = await LoadRecipeDtoFromRecipe(recipeDtos[index]);
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
                RecipeDto? returnedRecipeDto = await response.Content.ReadFromJsonAsync<RecipeDto>();

                if (returnedRecipeDto == null)
                {
                    const string errorMessage = "Failed to read returned recipe after saving it.";
                    Log.ForContext(_logProperty, _logDomainName).Error(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                returnedRecipeDto = await LoadRecipeDtoFromRecipe(returnedRecipeDto);

                return returnedRecipeDto;
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

    private async Task<RecipeDto> LoadRecipeDtoFromRecipe(RecipeDto recipeDto)
    {
        CategoryDto? categoryDto = await _categoryService.GetByIdAsync(recipeDto.CategoryId);

        if (categoryDto == null)
        {
            const string messageTemplate = "Failed to load recipe category from category service.";
            Log.ForContext(_logProperty, _logDomainName).Error(messageTemplate);
            throw new ArgumentNullException(nameof(recipeDto));
        }

        recipeDto.CategoryDto = categoryDto;
        recipeDto.IngredientMeasurementDtos.Sort((first, second) => first.SortOrder > second.SortOrder ? 1 : -1);

        return recipeDto;
    }
}
