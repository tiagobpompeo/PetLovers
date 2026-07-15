using PetLovers.Domain.Entities;

namespace PetLovers.API.Contracts;

public record PetDto(
    int Id, string Nome, string Especie, string Raca, string Sexo,
    DateOnly? DataNascimento, int? IdadeAnos, string Cor, decimal PesoKg,
    string? Microchip, string? FotoUrl,
    int TutorId, string TutorNome, List<VacinaDto> Vacinas)
{
    public static PetDto From(Pet p, DateOnly hoje) => new(
        p.Id, p.Nome, p.Especie.ToString(), p.Raca, p.Sexo.ToString(),
        p.DataNascimento, p.IdadeEmAnos(hoje), p.Cor, p.PesoKg,
        p.Microchip, p.FotoUrl,
        p.TutorId, p.Tutor?.Nome ?? "",
        p.Vacinas.Select(v => new VacinaDto(v.Id, v.Nome, v.DataAplicacao, v.ProximaDose, v.EstaPendente(hoje))).ToList());
}

public record VacinaDto(int Id, string Nome, DateOnly DataAplicacao, DateOnly? ProximaDose, bool Pendente);

public record PetInput(
    string Nome, Especie Especie, string? Raca, Sexo Sexo,
    DateOnly? DataNascimento, string? Cor, decimal PesoKg,
    string? Microchip, string? FotoUrl, int TutorId)
{
    public void ApplyTo(Pet pet)
    {
        pet.Nome = Nome.Trim();
        pet.Especie = Especie;
        pet.Raca = Raca?.Trim() ?? "";
        pet.Sexo = Sexo;
        pet.DataNascimento = DataNascimento;
        pet.Cor = Cor?.Trim() ?? "";
        pet.PesoKg = PesoKg;
        pet.Microchip = Microchip?.Trim();
        pet.FotoUrl = FotoUrl?.Trim();
        pet.TutorId = TutorId;
    }
}

public record VacinaInput(string Nome, DateOnly DataAplicacao, DateOnly? ProximaDose);

public record TutorDto(int Id, string Nome, string Cpf, string Telefone, string Email, string Endereco, int TotalPets);

public record TutorInput(string Nome, string? Cpf, string? Telefone, string? Email, string? Endereco)
{
    public void ApplyTo(Tutor tutor)
    {
        tutor.Nome = Nome.Trim();
        tutor.Cpf = Cpf?.Trim() ?? "";
        tutor.Telefone = Telefone?.Trim() ?? "";
        tutor.Email = Email?.Trim() ?? "";
        tutor.Endereco = Endereco?.Trim() ?? "";
    }
}

public record DashboardDto(int TotalPets, int TotalTutores, int VacinasPendentes, int AniversariantesDoMes);
