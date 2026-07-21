using System.Net.Http.Json;
using PetLovers.Mobile.Models;

namespace PetLovers.Mobile.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _http;

    public ApiService()
    {
        // Emulador Android: 10.0.2.2 alcança o localhost do host.
        // Dispositivo físico: localhost funciona via túnel USB (adb reverse tcp:5155 tcp:5155).
        var baseUrl = DeviceInfo.Platform == DevicePlatform.Android && DeviceInfo.DeviceType == DeviceType.Virtual
            ? "http://10.0.2.2:5155"
            : "http://localhost:5155";
        _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public Task<List<Pet>?> GetPetsAsync(string? busca = null) =>
        _http.GetFromJsonAsync<List<Pet>>(
            string.IsNullOrWhiteSpace(busca) ? "/api/pets" : $"/api/pets?busca={Uri.EscapeDataString(busca)}");

    public Task<Pet?> GetPetAsync(int id) =>
        _http.GetFromJsonAsync<Pet>($"/api/pets/{id}");

    public async Task CreatePetAsync(PetForm input) =>
        (await _http.PostAsJsonAsync("/api/pets", input)).EnsureSuccessStatusCode();

    public async Task UpdatePetAsync(int id, PetForm input) =>
        (await _http.PutAsJsonAsync($"/api/pets/{id}", input)).EnsureSuccessStatusCode();

    public async Task DeletePetAsync(int id) =>
        (await _http.DeleteAsync($"/api/pets/{id}")).EnsureSuccessStatusCode();

    public Task<List<Tutor>?> GetTutoresAsync() =>
        _http.GetFromJsonAsync<List<Tutor>>("/api/tutores");

    public Task<Dashboard?> GetDashboardAsync() =>
        _http.GetFromJsonAsync<Dashboard>("/api/dashboard");
}
