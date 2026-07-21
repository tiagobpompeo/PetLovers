using PetLovers.Mobile.Models;

namespace PetLovers.Mobile.Services;

// O contrato que as ViewModels enxergam. No app, quem implementa é o ApiService
// (rede real); num teste, um FakeApiService devolve dados fixos, sem rede.
public interface IApiService
{
    Task<List<Pet>?> GetPetsAsync(string? busca = null);
    Task<Pet?> GetPetAsync(int id);
    Task CreatePetAsync(PetForm input);
    Task UpdatePetAsync(int id, PetForm input);
    Task DeletePetAsync(int id);
    Task<List<Tutor>?> GetTutoresAsync();
    Task<Dashboard?> GetDashboardAsync();
}
