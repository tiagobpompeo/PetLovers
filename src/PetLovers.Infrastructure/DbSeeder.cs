using PetLovers.Domain.Entities;

namespace PetLovers.Infrastructure;

public static class DbSeeder
{
    public static void Seed(PetDbContext db)
    {
        if (db.Tutores.Any()) return;

        var hoje = DateOnly.FromDateTime(DateTime.Today);

        var maria = new Tutor
        {
            Nome = "Maria Silva", Cpf = "111.222.333-44",
            Telefone = "(11) 98888-0001", Email = "maria@email.com",
            Endereco = "Rua das Flores, 100 - São Paulo/SP"
        };
        var joao = new Tutor
        {
            Nome = "João Souza", Cpf = "555.666.777-88",
            Telefone = "(11) 97777-0002", Email = "joao@email.com",
            Endereco = "Av. Central, 250 - Campinas/SP"
        };

        var rex = new Pet
        {
            Nome = "Rex", Especie = Especie.Cachorro, Raca = "Labrador",
            Sexo = Sexo.Macho, DataNascimento = hoje.AddYears(-3),
            Cor = "Caramelo", PesoKg = 28.5m, Microchip = "985112003456789",
            Tutor = maria,
            Vacinas =
            [
                new Vacina { Nome = "V10", DataAplicacao = hoje.AddMonths(-11), ProximaDose = hoje.AddMonths(1) },
                new Vacina { Nome = "Antirrábica", DataAplicacao = hoje.AddMonths(-13), ProximaDose = hoje.AddDays(-10) }
            ]
        };
        var mimi = new Pet
        {
            Nome = "Mimi", Especie = Especie.Gato, Raca = "Siamês",
            Sexo = Sexo.Femea, DataNascimento = hoje.AddYears(-2),
            Cor = "Creme", PesoKg = 4.2m,
            Tutor = maria,
            Vacinas = [new Vacina { Nome = "V4 Felina", DataAplicacao = hoje.AddMonths(-2), ProximaDose = hoje.AddMonths(10) }]
        };
        var loro = new Pet
        {
            Nome = "Loro", Especie = Especie.Ave, Raca = "Calopsita",
            Sexo = Sexo.Macho, DataNascimento = hoje.AddYears(-1),
            Cor = "Amarelo", PesoKg = 0.09m,
            Tutor = joao
        };

        db.AddRange(rex, mimi, loro);
        db.SaveChanges();
    }
}
