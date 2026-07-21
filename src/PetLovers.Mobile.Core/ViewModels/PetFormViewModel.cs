using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetLovers.Mobile.Models;
using PetLovers.Mobile.Services;

namespace PetLovers.Mobile.ViewModels;

public partial class PetFormViewModel(IApiService api, INavigationService nav, IDialogService dialog) : ObservableObject
{
    public static readonly string[] Especies = ["Cachorro", "Gato", "Ave", "Roedor", "Reptil", "Outro"];
    public static readonly string[] Sexos = ["Macho", "Femea"];

    [ObservableProperty] private int _petId;
    [ObservableProperty] private string _titulo = "Novo Pet";
    [ObservableProperty] private string _nome = string.Empty;
    [ObservableProperty] private string _especie = "Cachorro";
    [ObservableProperty] private string _sexo = "Macho";
    [ObservableProperty] private string _raca = string.Empty;
    [ObservableProperty] private string _cor = string.Empty;
    [ObservableProperty] private DateTime _dataNascimento = DateTime.Today;
    [ObservableProperty] private string _pesoKg = string.Empty;
    [ObservableProperty] private string _microchip = string.Empty;
    [ObservableProperty] private Tutor? _tutorSelecionado;

    public ObservableCollection<Tutor> Tutores { get; } = [];

    [RelayCommand]
    private async Task InicializarAsync()
    {
        var tutores = await api.GetTutoresAsync() ?? [];
        Tutores.Clear();
        foreach (var t in tutores) Tutores.Add(t);

        if (PetId > 0)
        {
            Titulo = "Editar Pet";
            var pet = await api.GetPetAsync(PetId);
            if (pet is null) return;
            Nome = pet.Nome;
            Especie = pet.Especie;
            Sexo = pet.Sexo;
            Raca = pet.Raca;
            Cor = pet.Cor;
            DataNascimento = pet.DataNascimento?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Today;
            PesoKg = pet.PesoKg.ToString();
            Microchip = pet.Microchip ?? string.Empty;
            TutorSelecionado = Tutores.FirstOrDefault(t => t.Id == pet.TutorId);
        }
        else
        {
            TutorSelecionado = Tutores.FirstOrDefault();
        }
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        if (string.IsNullOrWhiteSpace(Nome))
        {
            await dialog.AlertAsync("Validação", "Informe o nome do pet.", "OK");
            return;
        }
        if (TutorSelecionado is null)
        {
            await dialog.AlertAsync("Validação", "Selecione um tutor.", "OK");
            return;
        }

        decimal.TryParse(PesoKg, out var peso);
        var input = new PetForm(
            Nome, Especie, Raca, Sexo,
            DateOnly.FromDateTime(DataNascimento), Cor, peso,
            string.IsNullOrWhiteSpace(Microchip) ? null : Microchip,
            null, TutorSelecionado.Id);

        try
        {
            if (PetId > 0) await api.UpdatePetAsync(PetId, input);
            else await api.CreatePetAsync(input);
            await nav.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await dialog.AlertAsync("Erro", $"Falha ao salvar: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private Task CancelarAsync() => nav.GoToAsync("..");
}
