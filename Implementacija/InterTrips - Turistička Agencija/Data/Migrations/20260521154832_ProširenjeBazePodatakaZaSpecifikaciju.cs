using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InterTrips___Turistička_Agencija.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProširenjeBazePodatakaZaSpecifikaciju : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rezervacija_Paket_PaketId",
                table: "Rezervacija");

            migrationBuilder.CreateTable(
                name: "Dobavljac",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VrstaUsluge = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KontaktOsoba = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aktivan = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dobavljac", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hoteli",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lokacija = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrojZvjezdica = table.Column<int>(type: "int", nullable: false),
                    UkupnoSoba = table.Column<int>(type: "int", nullable: false),
                    DostupnoSoba = table.Column<int>(type: "int", nullable: false),
                    CijenaPoNoci = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DostupneUsluge = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KontaktInformacije = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hoteli", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kupon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PopustProcenat = table.Column<int>(type: "int", nullable: false),
                    VaziDo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Iskoristen = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kupon", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Let",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Aviokompanija = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrojLeta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VrijemePolaska = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VrijemeDolaska = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipAviona = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UkupnoSjedista = table.Column<int>(type: "int", nullable: false),
                    SlobodnaSjedista = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Let", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogNotifikacija",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrimalacEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Naslov = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumSlanja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusSlanja = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DetaljiGreske = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PokusajBroj = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogNotifikacija", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RatePlacanja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlacanjeId = table.Column<int>(type: "int", nullable: false),
                    IznosRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RokZaUplatu = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumUplate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsUplaceno = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatePlacanja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RatePlacanja_Placanje_PlacanjeId",
                        column: x => x.PlacanjeId,
                        principalTable: "Placanje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // ✂️ OVDJE JE BIO migrationBuilder.InsertData ZA ULOGE - USPJEŠNO UKLONJEN DA IZBJEGNEMO SQU EXCEPTION

            migrationBuilder.CreateIndex(
                name: "IX_RatePlacanja_PlacanjeId",
                table: "RatePlacanja",
                column: "PlacanjeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rezervacija_Paket_PaketId",
                table: "Rezervacija",
                column: "PaketId",
                principalTable: "Paket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rezervacija_Paket_PaketId",
                table: "Rezervacija");

            migrationBuilder.DropTable(
                name: "Dobavljac");

            migrationBuilder.DropTable(
                name: "Hoteli");

            migrationBuilder.DropTable(
                name: "Kupon");

            migrationBuilder.DropTable(
                name: "Let");

            migrationBuilder.DropTable(
                name: "LogNotifikacija");

            migrationBuilder.DropTable(
                name: "RatePlacanja");


            migrationBuilder.AddForeignKey(
                name: "FK_Rezervacija_Paket_PaketId",
                table: "Rezervacija",
                column: "PaketId",
                principalTable: "Paket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}