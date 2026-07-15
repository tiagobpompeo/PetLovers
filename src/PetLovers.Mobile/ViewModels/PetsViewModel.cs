using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetLovers.Mobile.Models;
using PetLovers.Mobile.Services;

namespace PetLovers.Mobile.ViewModels;

public partial class PetsViewModel(ApiService api) : ObservableObject
{
    [ObservableProperty] private bool _carregando;
    [ObservableProperty] private string _busca = string.Empty;
    [ObservableProperty] private DashboardDto? _dashboard;

    public ObservableCollection<PetDto> Pets { get; } = [];

    [RelayCommand]
    private async Task CarregarAsync()
    {
        if (Carregando) return;
        Carregando = true;
        try
        {
            Dashboard = await api.GetDashboardAsync();
            var pets = await api.GetPetsAsync(Busca) ?? [];
            Pets.Clear();
            foreach (var pet in pets) Pets.Add(pet);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", $"Falha ao carregar: {ex.Message}", "OK");
        }
        finally
        {
            Carregando = false;
        }
    }

    [RelayCommand]
    private Task NovoPetAsync() => Shell.Current.GoToAsync("petform");

    [RelayCommand]
    private Task EditarPetAsync(PetDto pet) => Shell.Current.GoToAsync($"petform?petId={pet.Id}");

    [RelayCommand]
    private async Task ExcluirPetAsync(PetDto pet)
    {
        var confirma = await Shell.Current.DisplayAlert("Excluir", $"Excluir o pet \"{pet.Nome}\"?", "Sim", "Não");
        if (!confirma) return;
        await api.DeletePetAsync(pet.Id);
        await CarregarAsync();
    }

    partial void OnBuscaChanged(string value) => CarregarCommand.Execute(null);
}
