using PetLovers.Domain.Entities;

namespace PetLovers.UnitTests;

public class DomainTests
{
    private static readonly DateOnly Hoje = new(2026, 7, 15);

    [Fact]
    public void IdadeEmAnos_CalculaCorretamente()
    {
        var pet = new Pet { DataNascimento = new DateOnly(2023, 7, 10) };
        Assert.Equal(3, pet.IdadeEmAnos(Hoje));
    }

    [Fact]
    public void IdadeEmAnos_AntesDoAniversario_NaoContaAnoIncompleto()
    {
        var pet = new Pet { DataNascimento = new DateOnly(2023, 7, 20) };
        Assert.Equal(2, pet.IdadeEmAnos(Hoje));
    }

    [Fact]
    public void IdadeEmAnos_SemDataNascimento_RetornaNull()
    {
        var pet = new Pet();
        Assert.Null(pet.IdadeEmAnos(Hoje));
    }

    [Fact]
    public void Vacina_ComProximaDoseVencida_EstaPendente()
    {
        var vacina = new Vacina { ProximaDose = Hoje.AddDays(-1) };
        Assert.True(vacina.EstaPendente(Hoje));
    }

    [Fact]
    public void Vacina_ComProximaDoseFutura_NaoEstaPendente()
    {
        var vacina = new Vacina { ProximaDose = Hoje.AddMonths(2) };
        Assert.False(vacina.EstaPendente(Hoje));
    }

    [Fact]
    public void Vacina_SemProximaDose_NaoEstaPendente()
    {
        var vacina = new Vacina { ProximaDose = null };
        Assert.False(vacina.EstaPendente(Hoje));
    }

    [Fact]
    public void DiasAtraso_VacinaVencida_ContaOsDias()
    {
        var vacina = new Vacina { ProximaDose = Hoje.AddDays(-10) };
        Assert.Equal(10, vacina.DiasAtraso(Hoje));
    }

    [Fact]
    public void DiasAtraso_VenceHoje_RetornaZero()
    {
        var vacina = new Vacina { ProximaDose = Hoje };
        Assert.Equal(0, vacina.DiasAtraso(Hoje));
    }

    [Fact]
    public void DiasAtraso_DoseFutura_RetornaNull()
    {
        var vacina = new Vacina { ProximaDose = Hoje.AddMonths(3) };
        Assert.Null(vacina.DiasAtraso(Hoje));
    }

    [Fact]
    public void DiasAtraso_SemProximaDose_RetornaNull()
    {
        var vacina = new Vacina { ProximaDose = null };
        Assert.Null(vacina.DiasAtraso(Hoje));
    }
}
