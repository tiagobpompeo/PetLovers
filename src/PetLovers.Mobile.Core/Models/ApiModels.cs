namespace PetLovers.Mobile.Models;

public record Vacina(int Id, string Nome, DateOnly DataAplicacao, DateOnly? ProximaDose, bool Pendente, int? DiasAtraso);

public record Pet(
    int Id, string Nome, string Especie, string Raca, string Sexo,
    DateOnly? DataNascimento, int? IdadeAnos, string Cor, decimal PesoKg,
    string? Microchip, string? FotoUrl,
    int TutorId, string TutorNome, List<Vacina> Vacinas)
{
    public string Emoji => Especie switch
    {
        "Cachorro" => "🐶", "Gato" => "🐱", "Ave" => "🦜",
        "Roedor" => "🐹", "Reptil" => "🦎", _ => "🐾"
    };
    public string Subtitulo => $"{Especie}{(string.IsNullOrEmpty(Raca) ? "" : " · " + Raca)} — {TutorNome}";

    public bool TemAlerta => Vacinas.Any(v => v.Pendente);

    public string AlertaVacinas
    {
        get
        {
            var pendentes = Vacinas.Where(v => v.Pendente).ToList();
            return pendentes.Count switch
            {
                0 => "",
                1 => $"⚠️ {pendentes[0].Nome} atrasada há {pendentes[0].DiasAtraso} dia(s)",
                _ => $"⚠️ {pendentes.Count} vacinas atrasadas"
            };
        }
    }
}

// Dados que o app ENVIA ao criar/editar um pet (payload do formulário).
public record PetForm(
    string Nome, string Especie, string? Raca, string Sexo,
    DateOnly? DataNascimento, string? Cor, decimal PesoKg,
    string? Microchip, string? FotoUrl, int TutorId);

public record Tutor(int Id, string Nome, string Cpf, string Telefone, string Email, string Endereco, int TotalPets);

public record Dashboard(int TotalPets, int TotalTutores, int VacinasPendentes, int AniversariantesDoMes);
