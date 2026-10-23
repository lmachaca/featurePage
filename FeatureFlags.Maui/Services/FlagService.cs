using System.Net.Http.Json;
using FeatureFlags.Maui.Models;
 
namespace FeatureFlags.Maui.Services;

public class FlagService
{
    static readonly string BaseUrl = "http://localhost:5276";
    // Cliente HTTP con la dirección base y un límite de 5 segundos
    readonly HttpClient _http = new()
    {
        BaseAddress = new Uri(BaseUrl),
        Timeout = TimeSpan.FromSeconds(5)
    };

    // Hace GET http://.../api/flags y convierte el JSON en List<FeatureFlag>
    public async Task<List<FeatureFlag>> GetFlagsAsync()
        => await _http.GetFromJsonAsync<List<FeatureFlag>>("api/flags") ?? new();
}
