using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using PetLovers.API.Contracts;
using PetLovers.Domain.Entities;
using PetLovers.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Banco: SQL Server em produção; SQLite para desenvolvimento local rápido.
var useSqlServer = builder.Configuration.GetValue<bool>("Database:UseSqlServer");
builder.Services.AddDbContext<PetDbContext>(opt =>
{
    if (useSqlServer)
        opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
    else
        opt.UseSqlite(builder.Configuration.GetConnectionString("Sqlite") ?? "Data Source=petlovers.db");
});

builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => o.SwaggerDoc("v1", new() { Title = "PetLovers API", Version = "v1" }));
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// EF.IsDesignTime evita que este bloco rode durante os comandos "dotnet ef".
if (!EF.IsDesignTime)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<PetDbContext>();
    // SQL Server: aplica as migrations versionadas (como em produção).
    // SQLite: cria o schema direto, para uma demo rápida sem Docker.
    if (useSqlServer)
        db.Database.Migrate();
    else
        db.Database.EnsureCreated();
    DbSeeder.Seed(db);
}

app.UseSwagger();
app.UseSwaggerUI(o => o.SwaggerEndpoint("/swagger/v1/swagger.json", "PetLovers API v1"));
app.UseCors();

// Serve o site HTML (src/PetLovers.Web) na raiz.
var webRoot = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "PetLovers.Web"));
if (Directory.Exists(webRoot))
{
    var provider = new PhysicalFileProvider(webRoot);
    app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = provider });
    app.UseStaticFiles(new StaticFileOptions { FileProvider = provider });
}

var hoje = () => DateOnly.FromDateTime(DateTime.Today);

// ---------- Pets ----------
var pets = app.MapGroup("/api/pets").WithTags("Pets");

pets.MapGet("/", async (PetDbContext db, string? busca) =>
{
    var query = db.Pets.Include(p => p.Tutor).Include(p => p.Vacinas).AsNoTracking();
    if (!string.IsNullOrWhiteSpace(busca))
    {
        var b = busca.ToLower();
        query = query.Where(p =>
            p.Nome.ToLower().Contains(b) ||
            p.Raca.ToLower().Contains(b) ||
            (p.Microchip != null && p.Microchip.Contains(b)) ||
            p.Tutor!.Nome.ToLower().Contains(b));
    }
    var lista = await query.OrderBy(p => p.Nome).ToListAsync();
    return lista.Select(p => PetDto.From(p, hoje()));
});

pets.MapGet("/{id:int}", async (PetDbContext db, int id) =>
    await db.Pets.Include(p => p.Tutor).Include(p => p.Vacinas)
        .AsNoTracking().FirstOrDefaultAsync(p => p.Id == id)
        is { } pet ? Results.Ok(PetDto.From(pet, hoje())) : Results.NotFound());

pets.MapPost("/", async (PetDbContext db, PetInput input) =>
{
    if (string.IsNullOrWhiteSpace(input.Nome)) return Results.BadRequest("Nome é obrigatório.");
    if (!await db.Tutores.AnyAsync(t => t.Id == input.TutorId)) return Results.BadRequest("Tutor não encontrado.");

    var pet = new Pet();
    input.ApplyTo(pet);
    db.Pets.Add(pet);
    await db.SaveChangesAsync();
    return Results.Created($"/api/pets/{pet.Id}", new { pet.Id });
});

pets.MapPut("/{id:int}", async (PetDbContext db, int id, PetInput input) =>
{
    var pet = await db.Pets.FindAsync(id);
    if (pet is null) return Results.NotFound();
    if (!await db.Tutores.AnyAsync(t => t.Id == input.TutorId)) return Results.BadRequest("Tutor não encontrado.");

    input.ApplyTo(pet);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

pets.MapDelete("/{id:int}", async (PetDbContext db, int id) =>
{
    var pet = await db.Pets.FindAsync(id);
    if (pet is null) return Results.NotFound();
    db.Pets.Remove(pet);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// ---------- Vacinas ----------
pets.MapPost("/{id:int}/vacinas", async (PetDbContext db, int id, VacinaInput input) =>
{
    if (!await db.Pets.AnyAsync(p => p.Id == id)) return Results.NotFound();
    var vacina = new Vacina { PetId = id, Nome = input.Nome, DataAplicacao = input.DataAplicacao, ProximaDose = input.ProximaDose };
    db.Vacinas.Add(vacina);
    await db.SaveChangesAsync();
    return Results.Created($"/api/pets/{id}", new { vacina.Id });
});

// ---------- Tutores ----------
var tutores = app.MapGroup("/api/tutores").WithTags("Tutores");

tutores.MapGet("/", async (PetDbContext db) =>
    await db.Tutores.Include(t => t.Pets).AsNoTracking()
        .OrderBy(t => t.Nome)
        .Select(t => new TutorDto(t.Id, t.Nome, t.Cpf, t.Telefone, t.Email, t.Endereco, t.Pets.Count))
        .ToListAsync());

tutores.MapGet("/{id:int}", async (PetDbContext db, int id) =>
    await db.Tutores.Include(t => t.Pets).AsNoTracking().FirstOrDefaultAsync(t => t.Id == id)
        is { } t
        ? Results.Ok(new TutorDto(t.Id, t.Nome, t.Cpf, t.Telefone, t.Email, t.Endereco, t.Pets.Count))
        : Results.NotFound());

tutores.MapPost("/", async (PetDbContext db, TutorInput input) =>
{
    if (string.IsNullOrWhiteSpace(input.Nome)) return Results.BadRequest("Nome é obrigatório.");
    var tutor = new Tutor();
    input.ApplyTo(tutor);
    db.Tutores.Add(tutor);
    await db.SaveChangesAsync();
    return Results.Created($"/api/tutores/{tutor.Id}", new { tutor.Id });
});

tutores.MapPut("/{id:int}", async (PetDbContext db, int id, TutorInput input) =>
{
    var tutor = await db.Tutores.FindAsync(id);
    if (tutor is null) return Results.NotFound();
    input.ApplyTo(tutor);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

tutores.MapDelete("/{id:int}", async (PetDbContext db, int id) =>
{
    var tutor = await db.Tutores.FindAsync(id);
    if (tutor is null) return Results.NotFound();
    db.Tutores.Remove(tutor);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// ---------- Dashboard ----------
app.MapGet("/api/dashboard", async (PetDbContext db) =>
{
    var h = hoje();
    var mesAtual = h.Month;
    var petsDb = await db.Pets.Include(p => p.Vacinas).AsNoTracking().ToListAsync();
    return new DashboardDto(
        TotalPets: petsDb.Count,
        TotalTutores: await db.Tutores.CountAsync(),
        VacinasPendentes: petsDb.SelectMany(p => p.Vacinas).Count(v => v.EstaPendente(h)),
        AniversariantesDoMes: petsDb.Count(p => p.DataNascimento?.Month == mesAtual));
}).WithTags("Dashboard");

app.Run();
