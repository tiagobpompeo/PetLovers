namespace PetLovers.Domain.Entities;

public enum Especie { Cachorro, Gato, Ave, Roedor, Reptil, Outro }
public enum Sexo { Macho, Femea }

public class Pet
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public Especie Especie { get; set; }
    public string Raca { get; set; } = string.Empty;
    public Sexo Sexo { get; set; }
    public DateOnly? DataNascimento { get; set; }
    public string Cor { get; set; } = string.Empty;
    public decimal PesoKg { get; set; }
    public string? Microchip { get; set; }
    public string? FotoUrl { get; set; }

    public int TutorId { get; set; }
    public Tutor? Tutor { get; set; }

    public List<Vacina> Vacinas { get; set; } = [];

    public int? IdadeEmAnos(DateOnly hoje)
    {
        if (DataNascimento is null) return null;
        var idade = hoje.Year - DataNascimento.Value.Year;
        if (DataNascimento.Value > hoje.AddYears(-idade)) idade--;
        return idade < 0 ? 0 : idade;
    }
    
    public bool EhFilhote(DateOnly hoje) => IdadeEmAnos(hoje) is int i && i < 1;
}
