using System.Net.Http.Json;
using PetLovers.Mobile.Models;

namespace PetLovers.Mobile.Services;

public class ApiService
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

    public Task<List<PetDto>?> GetPetsAsync(string? busca = null) =>
        _http.GetFromJsonAsync<List<PetDto>>(
            string.IsNullOrWhiteSpace(busca) ? "/api/pets" : $"/api/pets?busca={Uri.EscapeDataString(busca)}");

    public Task<PetDto?> GetPetAsync(int id) =>
        _http.GetFromJsonAsync<PetDto>($"/api/pets/{id}");

    public async Task CreatePetAsync(PetInput input) =>
        (await _http.PostAsJsonAsync("/api/pets", input)).EnsureSuccessStatusCode();

    public async Task UpdatePetAsync(int id, PetInput input) =>
        (await _http.PutAsJsonAsync($"/api/pets/{id}", input)).EnsureSuccessStatusCode();

    public async Task DeletePetAsync(int id) =>
        (await _http.DeleteAsync($"/api/pets/{id}")).EnsureSuccessStatusCode();

    public Task<List<TutorDto>?> GetTutoresAsync() =>
        _http.GetFromJsonAsync<List<TutorDto>>("/api/tutores");

    public Task<DashboardDto?> GetDashboardAsync() =>
        _http.GetFromJsonAsync<DashboardDto>("/api/dashboard");
}
