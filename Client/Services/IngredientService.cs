using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Logging;
using BlazingRecept.Shared;
using BlazingRecept.Shared.Dto;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net;
using System.Net.Http.Json;
using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Client.Services;

public class IngredientService : IIngredientService
{
    private static readonly string _apiAddress = "api/ingredients";

    private readonly HttpClient _publicHttpClient;
    private readonly HttpClient _authenticatedHttpClient;

    private readonly ICategoryService _categoryService;

    public IngredientService(IHttpClientFactory httpClientFactory, ICategoryService categoryService)
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
            Log.Error(exception, $"Failed while checking existence of ingredient with name: {name}");
        }

        return false;
    }

    public async Task<IngredientDto?> GetByIdAsync(Guid id)
    {
        try
        {
            HttpResponseMessage response = await _publicHttpClient.GetAsync(_apiAddress + $"/{id}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadFromJsonAsync<IngredientDto>();
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, $"Failed while fetching ingredient with id: {id}");
        }

        return null;
    }

    public async Task<IReadOnlyList<IngredientDto>?> GetAllAsync()
    {
        try
        {
            HttpResponseMessage response = await _publicHttpClient.GetAsync(_apiAddress);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                IReadOnlyList<IngredientDto>? ingredientDtos = await response.Content.ReadFromJsonAsync<IReadOnlyList<IngredientDto>>();

                if (ingredientDtos == null)
                {
                    throw new NullReferenceException("Failed because fetched ingredient list is null.");
                }

                return ingredientDtos;
            }
        }
        catch (Exception exception)
        {
            const string errorMessage = "Failed while fetching all ingredients.";
            Log.Error(exception, errorMessage);
        }

        return null;
    }

    public async Task<IReadOnlyList<IngredientCollectionTypeDto>?> GetAllSortedAsync()
    {
        IReadOnlyList<IngredientDto>? ingredientDtos = await GetAllAsync();

        Contracts.LogAndThrowWhenNull(ingredientDtos, "Failed while fetching all ingredients sorted because fetched and unsorted ingredient dto list is null.");

        IReadOnlyList<CategoryDto>? categoryDtos = await _categoryService.GetAllOfTypeAsync(CategoryType.Ingredient);

        Contracts.LogAndThrowWhenNull(categoryDtos, "Failed while fetching all ingredients sorted because fetched category dto list is null.");

        List<IngredientCollectionTypeDto> ingredientCollectionTypes = new();

        foreach (CategoryDto CategoryDto in categoryDtos)
        {
            IngredientCollectionTypeDto ingredientCollectionTypeDto = new();
            ingredientCollectionTypeDto.Name = CategoryDto.Name;

            ingredientCollectionTypes.Add(ingredientCollectionTypeDto);
        }

        foreach (IngredientDto ingredientDto in ingredientDtos)
        {
            ingredientCollectionTypes[ingredientDto.CategoryDto.SortOrder].Ingredients.Add(ingredientDto);
        }

        foreach (IngredientCollectionTypeDto ingredientCollectionTypeDto in ingredientCollectionTypes)
        {
            ingredientCollectionTypeDto.Ingredients = ingredientCollectionTypeDto.Ingredients
                .OrderBy(ingredientDto => ingredientDto.Name)
                .ToList();
        }

        return ingredientCollectionTypes;
    }

    public async Task<IngredientDto?> SaveAsync(IngredientDto ingredientDto)
    {
        try
        {
            HttpResponseMessage response = await _authenticatedHttpClient.PostAsJsonAsync(_apiAddress, ingredientDto);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadFromJsonAsync<IngredientDto>();
            }
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
        catch (Exception exception)
        {
            Log.Error(exception, $"Failed while saving ingredient: {ingredientDto}");
        }

        return null;
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
            Log.Error(exception, $"Failed while deleting ingredient with id: {id}");
        }

        return false;
    }
}
