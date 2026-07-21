using PetLovers.Mobile.Models;
using PetLovers.Mobile.Services;
using PetLovers.Mobile.ViewModels;

namespace PetLovers.UnitTests;

// ── Dublês de teste ────────────────────────────────────────────────
// Stub: implementa IApiService devolvendo dados fixos, sem rede.
class FakeApiService : IApiService
{
    public int DeleteChamadoComId { get; private set; } = -1;

    public Task<List<Pet>?> GetPetsAsync(string? busca = null) =>
        Task.FromResult<List<Pet>?>(
        [
            new Pet(1, "Rex", "Cachorro", "Labrador", "Macho", null, 3, "", 28.5m, null, null, 1, "Maria", []),
            new Pet(2, "Mimi", "Gato", "Siamês", "Femea", null, 2, "", 4.2m, null, null, 1, "Maria", [])
        ]);

    public Task<Dashboard?> GetDashboardAsync() =>
        Task.FromResult<Dashboard?>(new Dashboard(2, 1, 0, 0));

    // Mock: registra que foi chamado, para o teste verificar a interação.
    public Task DeletePetAsync(int id) { DeleteChamadoComId = id; return Task.CompletedTask; }

    // O restante não é usado nestes testes.
    public Task<Pet?> GetPetAsync(int id) => throw new NotImplementedException();
    public Task CreatePetAsync(PetForm input) => throw new NotImplementedException();
    public Task UpdatePetAsync(int id, PetForm input) => throw new NotImplementedException();
    public Task<List<Tutor>?> GetTutoresAsync() => throw new NotImplementedException();
}

// Dublês no-op para navegação e diálogo (sem UI).
class FakeNavigation : INavigationService
{
    public Task GoToAsync(string route) => Task.CompletedTask;
}

class FakeDialog : IDialogService
{
    public Task AlertAsync(string t, string m, string c) => Task.CompletedTask;
    public Task<bool> ConfirmAsync(string t, string m, string a, string c) => Task.FromResult(true);
}

// ── Os testes ──────────────────────────────────────────────────────
public class PetsViewModelTests
{
    private static PetsViewModel CriarViewModel(FakeApiService api) =>
        new(api, new FakeNavigation(), new FakeDialog());

    [Fact]
    public async Task Carregar_PreencheAListaComOsPetsDaApi()
    {
        var vm = CriarViewModel(new FakeApiService());

        await vm.CarregarCommand.ExecuteAsync(null);

        Assert.Equal(2, vm.Pets.Count);
        Assert.Equal("Rex", vm.Pets[0].Nome);
    }

    [Fact]
    public async Task Carregar_PreencheODashboard()
    {
        var vm = CriarViewModel(new FakeApiService());

        await vm.CarregarCommand.ExecuteAsync(null);

        Assert.Equal(2, vm.Dashboard!.TotalPets);
    }

    [Fact]
    public async Task Carregar_AoTerminar_DesligaOIndicadorDeCarregamento()
    {
        var vm = CriarViewModel(new FakeApiService());

        await vm.CarregarCommand.ExecuteAsync(null);

        Assert.False(vm.Carregando);
    }

    [Fact]
    public async Task Excluir_ChamaADeleteNaApiComOIdCerto()
    {
        var api = new FakeApiService();
        var vm = CriarViewModel(api);
        var pet = new Pet(3, "Thor", "Cachorro", "", "Macho", null, 2, "", 11m, null, null, 1, "Maria", []);

        await vm.ExcluirPetCommand.ExecuteAsync(pet);

        Assert.Equal(3, api.DeleteChamadoComId);
    }
}
