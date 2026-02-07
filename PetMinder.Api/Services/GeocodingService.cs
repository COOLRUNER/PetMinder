using System.Globalization;
using System.Text.Json;
using PetMinder.Shared.DTO;

namespace WebApplication1.Services.Interfaces;

public class GeocodingService : IGeocodingService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GeocodingService> _logger;
    
    private const string UserAgent = "PetMinderThesisApp/1.0 (petminder.dev.test@gmail.com)";
    
    public GeocodingService(IHttpClientFactory httpClientFactory, ILogger<GeocodingService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<(double Lat, double Lon)?> GetCoordinatesAsync(CreateAddressDTO address)
    {
        var query = $"{address.Street}, {address.City}, Poland";
        string url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(query)}&format=json&limit=1";

        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);

        try
        {
            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Nominatim API request failed with status {StatusCode} for address {Query}",
                    response.StatusCode, query);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content) || content == "[]")
            {
                _logger.LogWarning("Nominatim API returned no results for address {Query}", query);
                return null;
            }

            using (JsonDocument doc = JsonDocument.Parse(content))
            {
                var firstResult = doc.RootElement.EnumerateArray().FirstOrDefault();
                if (firstResult.TryGetProperty("lat", out var latProp) &&
                    firstResult.TryGetProperty("lon", out var lonProp))
                {
                    double lat = 0;
                    double lon = 0;
                    bool latSuccess = false;
                    bool lonSuccess = false;

                    if (latProp.ValueKind == JsonValueKind.Number)
                    {
                        latSuccess = latProp.TryGetDouble(out lat);
                    } 
                    else if (latProp.ValueKind == JsonValueKind.String)
                    {
                        latSuccess = double.TryParse(latProp.GetString(), NumberStyles.Float,
                            CultureInfo.InvariantCulture, out lat);
                    } 
                    
                    if (lonProp.ValueKind == JsonValueKind.Number)
                    {
                        lonSuccess = lonProp.TryGetDouble(out lon);
                    } 
                    else if (lonProp.ValueKind == JsonValueKind.String)
                    {
                        lonSuccess = double.TryParse(lonProp.GetString(), NumberStyles.Float,
                            CultureInfo.InvariantCulture, out lon);
                    }

                    if (latSuccess && lonSuccess)
                    {
                        return (lat, lon);
                    }

                }
            }

            _logger.LogWarning("Failed to parse lat/lon from Nominatim response for {Query}", query);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Nominatim API for address {Query}", query);
            return null;
        }
    }
}