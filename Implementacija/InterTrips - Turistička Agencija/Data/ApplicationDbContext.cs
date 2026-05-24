using InterTrips___Turistička_Agencija.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InterTrips___Turistička_Agencija.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Korisnik> Korisnici => Set<Korisnik>();
        public DbSet<Destinacija> Destinacije => Set<Destinacija>();
        public DbSet<Paket> Paketi => Set<Paket>();
        public DbSet<UslugaPaketa> UslugePaketa => Set<UslugaPaketa>();
        public DbSet<Rezervacija> Rezervacije => Set<Rezervacija>();
        public DbSet<Putnik> Putnici => Set<Putnik>();
        public DbSet<Placanje> Placanja => Set<Placanje>();
        public DbSet<Notifikacija> Notifikacije => Set<Notifikacija>();
        public DbSet<PlanPutovanja> PlanoviPutovanja => Set<PlanPutovanja>();
        public DbSet<StavkaPlana> StavkePlana => Set<StavkaPlana>();
        public DbSet<PlanPutovanjaTemplate> PlanoviPutovanjaTemplate => Set<PlanPutovanjaTemplate>();
        public DbSet<StavkaPlanaTemplate> StavkePlanaTemplate => Set<StavkaPlanaTemplate>();
        public DbSet<AgentPaket> AgentPaketi { get; set; }

        public DbSet<Hotel> Hoteli => Set<Hotel>();
        public DbSet<Let> Letovi => Set<Let>();
        public DbSet<Dobavljac> Dobavljaci => Set<Dobavljac>();
        public DbSet<Kupon> Kupon => Set<Kupon>();
        public DbSet<RataPlacanja> RatePlacanja => Set<RataPlacanja>();
        public DbSet<LogNotifikacija> LogoviNotifikacija => Set<LogNotifikacija>();
        public DbSet<TerminPutovanja> TerminPutovanja { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Korisnik>().ToTable("Korisnik");
            modelBuilder.Entity<Destinacija>().ToTable("Destinacija");
            modelBuilder.Entity<Paket>().ToTable("Paket");
            modelBuilder.Entity<UslugaPaketa>().ToTable("UslugaPaketa");
            modelBuilder.Entity<Rezervacija>().ToTable("Rezervacija");
            modelBuilder.Entity<Putnik>().ToTable("Putnik");
            modelBuilder.Entity<Placanje>().ToTable("Placanje");
            modelBuilder.Entity<Notifikacija>().ToTable("Notifikacija");
            modelBuilder.Entity<PlanPutovanja>().ToTable("PlanPutovanja");
            modelBuilder.Entity<StavkaPlana>().ToTable("StavkaPlana");
            modelBuilder.Entity<AgentPaket>().ToTable("AgentPaket");

            modelBuilder.Entity<RataPlacanja>().Property(r => r.IznosRate).HasPrecision(18, 2);
            modelBuilder.Entity<Let>().ToTable("Let");
            modelBuilder.Entity<Dobavljac>().ToTable("Dobavljac");
            modelBuilder.Entity<Kupon>().ToTable("Kupon");
            modelBuilder.Entity<LogNotifikacija>().ToTable("LogNotifikacija");

            modelBuilder.Entity<PlanPutovanjaTemplate>()
                .HasMany(p => p.Stavke)
                .WithOne(s => s.PlanPutovanjaTemplate!)
                .HasForeignKey(s => s.PlanPutovanjaTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

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

            modelBuilder.Entity<PlanPutovanja>()
                .HasMany(p => p.StavkePlana)
                .WithOne(s => s.PlanPutovanja!)
                .HasForeignKey(s => s.PlanPutovanjaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Placanje>()
                .HasMany(p => p.Rate)
                .WithOne(r => r.Placanje!)
                .HasForeignKey(r => r.PlacanjeId)
                .OnDelete(DeleteBehavior.Cascade);

            string adminRoleId = "1b63ef27-996b-4b13-98db-00f7e4b9bc10";
            string agentRoleId = "2c74fa38-885b-3b12-87cb-11e8e5c8cd21";
            string klijentRoleId = "3d85fb49-774b-2b11-76da-22f9e6d9de32";

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = agentRoleId, Name = "Agent", NormalizedName = "AGENT" },
                new IdentityRole { Id = klijentRoleId, Name = "Klijent", NormalizedName = "KLIJENT" }
            );

            modelBuilder.Entity<Rezervacija>()
                .HasOne(r => r.Paket)
                .WithMany(p => p.Rezervacije)
                .HasForeignKey(r => r.PaketId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Paket>()
                .ToTable(tb => tb.HasTrigger("tr_Paketi_Trigger"));
        }
    }
}