using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterTrips___Turistička_Agencija.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInterTripsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Destinacija",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Drzava = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Kategorija = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Opis = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destinacija", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Korisnik",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Uloga = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnik", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Paket",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CijenaOd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrajanjeNoci = table.Column<int>(type: "int", nullable: false),
                    DestinacijaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Paket_Destinacija_DestinacijaId",
                        column: x => x.DestinacijaId,
                        principalTable: "Destinacija",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifikacija",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    Poruka = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Vrijeme = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Poslano = table.Column<bool>(type: "bit", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifikacija", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifikacija_Korisnik_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rezervacija",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatumPolaska = table.Column<DateOnly>(type: "date", nullable: false),
                    DatumPovratka = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaketId = table.Column<int>(type: "int", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezervacija", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rezervacija_Korisnik_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rezervacija_Paket_PaketId",
                        column: x => x.PaketId,
                        principalTable: "Paket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UslugaPaketa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    PaketId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UslugaPaketa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UslugaPaketa_Paket_PaketId",
                        column: x => x.PaketId,
                        principalTable: "Paket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Placanje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Metoda = table.Column<int>(type: "int", nullable: false),
                    Iznos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VrijemePlacanja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RezervacijaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Placanje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Placanje_Rezervacija_RezervacijaId",
                        column: x => x.RezervacijaId,
                        principalTable: "Rezervacija",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanPutovanja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Napomena = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: true),
                    RezervacijaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanPutovanja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanPutovanja_Rezervacija_RezervacijaId",
                        column: x => x.RezervacijaId,
                        principalTable: "Rezervacija",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Putnik",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    BrojPasosa = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Drzavljanstvo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    PosebniZahtjevi = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    RezervacijaId = table.Column<int>(type: "int", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Putnik", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Putnik_Korisnik_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnik",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Putnik_Rezervacija_RezervacijaId",
                        column: x => x.RezervacijaId,
                        principalTable: "Rezervacija",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StavkaPlana",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanPutovanjaId = table.Column<int>(type: "int", nullable: false),
                    RedniBroj = table.Column<int>(type: "int", nullable: false),
                    Naziv = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    DatumVrijeme = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StavkaPlana", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StavkaPlana_PlanPutovanja_PlanPutovanjaId",
                        column: x => x.PlanPutovanjaId,
                        principalTable: "PlanPutovanja",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifikacija_KorisnikId",
                table: "Notifikacija",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Paket_DestinacijaId",
                table: "Paket",
                column: "DestinacijaId");

            migrationBuilder.CreateIndex(
                name: "IX_Placanje_RezervacijaId",
                table: "Placanje",
                column: "RezervacijaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanPutovanja_RezervacijaId",
                table: "PlanPutovanja",
                column: "RezervacijaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Putnik_KorisnikId",
                table: "Putnik",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Putnik_RezervacijaId",
                table: "Putnik",
                column: "RezervacijaId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacija_KorisnikId",
                table: "Rezervacija",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacija_PaketId",
                table: "Rezervacija",
                column: "PaketId");

            migrationBuilder.CreateIndex(
                name: "IX_StavkaPlana_PlanPutovanjaId",
                table: "StavkaPlana",
                column: "PlanPutovanjaId");

            migrationBuilder.CreateIndex(
                name: "IX_UslugaPaketa_PaketId",
                table: "UslugaPaketa",
                column: "PaketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifikacija");

            migrationBuilder.DropTable(
                name: "Placanje");

            migrationBuilder.DropTable(
                name: "Putnik");

            migrationBuilder.DropTable(
                name: "StavkaPlana");

            migrationBuilder.DropTable(
                name: "UslugaPaketa");

            migrationBuilder.DropTable(
                name: "PlanPutovanja");

            migrationBuilder.DropTable(
                name: "Rezervacija");

            migrationBuilder.DropTable(
                name: "Korisnik");

            migrationBuilder.DropTable(
                name: "Paket");

            migrationBuilder.DropTable(
                name: "Destinacija");
        }
    }
}
