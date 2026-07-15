namespace PetLovers.Mobile.Models;

public record VacinaDto(int Id, string Nome, DateOnly DataAplicacao, DateOnly? ProximaDose, bool Pendente);

public record PetDto(
    int Id, string Nome, string Especie, string Raca, string Sexo,
    DateOnly? DataNascimento, int? IdadeAnos, string Cor, decimal PesoKg,
    string? Microchip, string? FotoUrl,
    int TutorId, string TutorNome, List<VacinaDto> Vacinas)
{
    public string Emoji => Especie switch
    {
        "Cachorro" => "🐶", "Gato" => "🐱", "Ave" => "🦜",
        "Roedor" => "🐹", "Reptil" => "🦎", _ => "🐾"
    };
    public string Subtitulo => $"{Especie}{(string.IsNullOrEmpty(Raca) ? "" : " · " + Raca)} — {TutorNome}";
}

public record PetInput(
    string Nome, string Especie, string? Raca, string Sexo,
    DateOnly? DataNascimento, string? Cor, decimal PesoKg,
    string? Microchip, string? FotoUrl, int TutorId);

public record TutorDto(int Id, string Nome, string Cpf, string Telefone, string Email, string Endereco, int TotalPets);

public record DashboardDto(int TotalPets, int TotalTutores, int VacinasPendentes, int AniversariantesDoMes);
