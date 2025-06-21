using Microsoft.EntityFrameworkCore;
using TrabajadoresPrueba.Models;

namespace TrabajadoresPrueba.Data;

public class PruebaContext : DbContext
{
    public DbSet<Trabajador> Trabajador{ get; set; }
    public DbSet<Departamento> Departamento { get; set; }
    public DbSet<Provincia> Provincia { get; set; }
    public DbSet<Distrito> Distrito { get; set; }
    public DbSet<TrabajadorDTO> TrabajadorDTO { get; set; }
    public PruebaContext(DbContextOptions<PruebaContext> options): base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Trabajador>().ToTable("Trabajadores");

        modelBuilder.Entity<TrabajadorDTO>().HasNoKey();
        
        modelBuilder.Entity<Departamento>()
            .HasMany(d => d.Provincias)
            .WithOne(p => p.Departamento)
            .HasForeignKey(p => p.IdDepartamento);

        modelBuilder.Entity<Provincia>()
            .HasMany(p => p.Distritos)
            .WithOne(d => d.Provincia)
            .HasForeignKey(d => d.IdProvincia);

        modelBuilder.Entity<Provincia>()
            .HasMany(p => p.Trabajadores)
            .WithOne(t => t.Provincia)
            .HasForeignKey(t => t.IdProvincia);

        modelBuilder.Entity<Departamento>()
            .HasMany(d => d.Trabajadores)
            .WithOne(t => t.Departamento)
            .HasForeignKey(t => t.IdDepartamento);

        modelBuilder.Entity<Distrito>()
            .HasMany(d => d.Trabajadores)
            .WithOne(t => t.Distrito)
            .HasForeignKey(t => t.IdDistrito);
    }
}
