using BlazingRecept.Client.Services.Interfaces;
using BlazingRecept.Logging;
using BlazingRecept.Shared.Dto;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazingRecept.Client.Services;

public class IngredientSearchService : IIngredientSearchService
{
    private readonly HttpClient _httpClient;
    private static readonly Dictionary<string, string> _englishToSwedishTranslations = new()
    {
        // Common ingredients translations
        { "chicken", "kyckling" },
        { "beef", "nötkött" },
        { "pork", "fläsk" },
        { "fish", "fisk" },
        { "salmon", "lax" },
        { "egg", "ägg" },
        { "eggs", "ägg" },
        { "milk", "mjölk" },
        { "cheese", "ost" },
        { "bread", "bröd" },
        { "rice", "ris" },
        { "pasta", "pasta" },
        { "potato", "potatis" },
        { "tomato", "tomat" },
        { "onion", "lök" },
        { "garlic", "vitlök" },
        { "carrot", "morot" },
        { "apple", "äpple" },
        { "banana", "banan" },
        { "orange", "apelsin" },
        { "strawberry", "jordgubbe" },
        { "butter", "smör" },
        { "oil", "olja" },
        { "sugar", "socker" },
        { "salt", "salt" },
        { "pepper", "peppar" },
        { "flour", "mjöl" },
        { "water", "vatten" },
        { "cream", "grädde" }
    };

    public IngredientSearchService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("OpenFoodFacts");
    }

    public async Task<IReadOnlyList<IngredientSearchResultDto>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            return Array.Empty<IngredientSearchResultDto>();
        }

        try
        {
            var results = new List<IngredientSearchResultDto>();
            
            // Try searching in English first
            var englishResults = await SearchOpenFoodFactsAsync(query, "en");
            results.AddRange(englishResults);

            // If query might be Swedish, also try Swedish search
            string swedishQuery = TranslateToSwedish(query);
            if (swedishQuery != query)
            {
                var swedishResults = await SearchOpenFoodFactsAsync(swedishQuery, "sv");
                results.AddRange(swedishResults);
            }

            // Remove duplicates and limit results
            return results
                .GroupBy(r => r.Name.ToLowerInvariant())
                .Select(g => g.First())
                .Take(10)
                .ToList();
        }
        catch (Exception exception)
        {
            Log.Error(exception, $"Failed while searching for ingredient: {query}");
            return Array.Empty<IngredientSearchResultDto>();
        }
    }

    private async Task<List<IngredientSearchResultDto>> SearchOpenFoodFactsAsync(string query, string language)
    {
        try
        {
            var url = $"cgi/search.pl?search_terms={Uri.EscapeDataString(query)}&search_simple=1&action=process&json=1&page_size=10&fields=product_name,product_name_en,product_name_sv,nutriments";
            
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return new List<IngredientSearchResultDto>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var searchResponse = JsonSerializer.Deserialize<OpenFoodFactsSearchResponse>(content, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });

            if (searchResponse?.Products == null)
            {
                return new List<IngredientSearchResultDto>();
            }

            return searchResponse.Products
                .Where(p => !string.IsNullOrWhiteSpace(p.ProductName) && p.Nutriments != null)
                .Select(p => new IngredientSearchResultDto
                {
                    Name = p.ProductName ?? string.Empty,
                    NameEnglish = p.ProductNameEn ?? p.ProductName ?? string.Empty,
                    NameSwedish = p.ProductNameSv ?? p.ProductName ?? string.Empty,
                    Fat = p.Nutriments?.Fat100g ?? 0,
                    Carbohydrates = p.Nutriments?.Carbohydrates100g ?? 0,
                    Protein = p.Nutriments?.Proteins100g ?? 0,
                    Calories = p.Nutriments?.EnergyKcal100g ?? 0
                })
                .Where(r => r.Calories > 0 || r.Fat > 0 || r.Carbohydrates > 0 || r.Protein > 0)
                .ToList();
        }
        catch (Exception exception)
        {
            Log.Error(exception, $"Failed while searching Open Food Facts for: {query} in language: {language}");
            return new List<IngredientSearchResultDto>();
        }
    }

    private string TranslateToSwedish(string englishWord)
    {
        var lowerQuery = englishWord.ToLowerInvariant();
        
        foreach (var translation in _englishToSwedishTranslations)
        {
            if (lowerQuery.Contains(translation.Key))
            {
                return translation.Value;
            }
        }
        
        return englishWord;
    }

    // Open Food Facts API response models
    private class OpenFoodFactsSearchResponse
    {
        [JsonPropertyName("products")]
        public List<OpenFoodFactsProduct>? Products { get; set; }
    }

    private class OpenFoodFactsProduct
    {
        [JsonPropertyName("product_name")]
        public string? ProductName { get; set; }

        [JsonPropertyName("product_name_en")]
        public string? ProductNameEn { get; set; }

        [JsonPropertyName("product_name_sv")]
        public string? ProductNameSv { get; set; }

        [JsonPropertyName("nutriments")]
        public OpenFoodFactsNutriments? Nutriments { get; set; }
    }

    private class OpenFoodFactsNutriments
    {
        [JsonPropertyName("fat_100g")]
        public double? Fat100g { get; set; }

        [JsonPropertyName("carbohydrates_100g")]
        public double? Carbohydrates100g { get; set; }

        [JsonPropertyName("proteins_100g")]
        public double? Proteins100g { get; set; }

        [JsonPropertyName("energy-kcal_100g")]
        public double? EnergyKcal100g { get; set; }
    }
}
