namespace PetLovers.Domain.Entities;

public class Vacina
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateOnly DataAplicacao { get; set; }
    public DateOnly? ProximaDose { get; set; }

    public int PetId { get; set; }
    public Pet? Pet { get; set; }

    public bool EstaPendente(DateOnly hoje) => ProximaDose is not null && ProximaDose <= hoje;
}
