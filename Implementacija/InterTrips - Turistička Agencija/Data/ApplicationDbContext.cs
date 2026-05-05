using InterTrips___Turistička_Agencija.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InterTrips___Turistička_Agencija.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Korisnik> Korisnici => Set<Korisnik>();
    public DbSet<Destinacija> Destinacije => Set<Destinacija>();
    public DbSet<Paket> Paketi => Set<Paket>();
    public DbSet<UslugaPaketa> UslugePaketa => Set<UslugaPaketa>();
    public DbSet<Rezervacija> Rezervacije => Set<Rezervacija>();
    public DbSet<Putnik> Putnici => Set<Putnik>();
    public DbSet<Placanje> Placanja => Set<Placanje>();
    public DbSet<Notifikacija> Notifikacije => Set<Notifikacija>();

    public DbSet<StavkaPlana> StavkePlana => Set<StavkaPlana>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Korisnik>().ToTable("Korisnik");
        modelBuilder.Entity<Destinacija>().ToTable("Destinacija");
        modelBuilder.Entity<Paket>().ToTable("Paket");
        modelBuilder.Entity<UslugaPaketa>().ToTable("UslugaPaketa");
        modelBuilder.Entity<Rezervacija>().ToTable("Rezervacija");
        modelBuilder.Entity<Putnik>().ToTable("Putnik");
        modelBuilder.Entity<Placanje>().ToTable("Placanje");
        modelBuilder.Entity<Notifikacija>().ToTable("Notifikacija");
        modelBuilder.Entity<PlanPutovanja>().ToTable("PlanPutovanja");


        modelBuilder.Entity<Rezervacija>()
            .HasOne(r => r.Placanje)
            .WithOne(p => p.Rezervacija!)
            .HasForeignKey<Placanje>(p => p.RezervacijaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Rezervacija>()
            .HasOne(r => r.PlanPutovanja)
            .WithOne(p => p.Rezervacija!)
            .HasForeignKey<PlanPutovanja>(p => p.RezervacijaId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}