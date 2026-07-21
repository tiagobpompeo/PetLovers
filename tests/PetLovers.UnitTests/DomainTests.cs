using PetLovers.Domain.Entities;

namespace PetLovers.UnitTests;

public class DomainTests
{
    private static readonly DateOnly Hoje = new(2026, 7, 15);

    // [Theory] roda o MESMO teste com vários conjuntos de dados.
    // Cada [InlineData] vira um caso independente no relatório.
    [Theory]
    [InlineData(-4, true)]    // nasceu há 4 meses  → filhote
    [InlineData(-11, true)]   // 11 meses           → ainda filhote
    [InlineData(-12, false)]  // 1 ano exato        → não é mais
    [InlineData(-36, false)]  // 3 anos             → adulto
    public void EhFilhote_ConformeIdade(int mesesAtras, bool esperado)
    {
        var pet = new Pet { DataNascimento = Hoje.AddMonths(mesesAtras) };
        Assert.Equal(esperado, pet.EhFilhote(Hoje));
    }

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
    
    
    [Fact]
    public void EhFilhote_PetComMenosDeUmAno_RetornaTrue()
    {
        var pet = new Pet { DataNascimento = Hoje.AddMonths(-4) };
        Assert.True(pet.EhFilhote(Hoje));
    }

    [Fact]
    public void EhFilhote_PetAdulto_RetornaFalse()
    {
        var pet = new Pet { DataNascimento = new DateOnly(2023, 7, 10) };
        Assert.False(pet.EhFilhote(Hoje));
    }

    [Fact]
    public void EhFilhote_SemDataNascimento_RetornaFalse()
    {
        var pet = new Pet();
        Assert.False(pet.EhFilhote(Hoje));
    }
}
