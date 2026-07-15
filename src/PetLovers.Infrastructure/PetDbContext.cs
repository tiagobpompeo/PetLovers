using Microsoft.EntityFrameworkCore;
using PetLovers.Domain.Entities;

namespace PetLovers.Infrastructure;

public class PetDbContext(DbContextOptions<PetDbContext> options) : DbContext(options)
{
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<Tutor> Tutores => Set<Tutor>();
    public DbSet<Vacina> Vacinas => Set<Vacina>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tutor>(e =>
        {
            e.Property(t => t.Nome).HasMaxLength(150).IsRequired();
            e.Property(t => t.Cpf).HasMaxLength(14);
            e.HasIndex(t => t.Cpf).IsUnique();
        });

        modelBuilder.Entity<Pet>(e =>
        {
            e.Property(p => p.Nome).HasMaxLength(100).IsRequired();
            e.Property(p => p.PesoKg).HasPrecision(6, 2);
            e.HasOne(p => p.Tutor)
                .WithMany(t => t.Pets)
                .HasForeignKey(p => p.TutorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Vacina>(e =>
        {
            e.Property(v => v.Nome).HasMaxLength(100).IsRequired();
            e.HasOne(v => v.Pet)
                .WithMany(p => p.Vacinas)
                .HasForeignKey(v => v.PetId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
