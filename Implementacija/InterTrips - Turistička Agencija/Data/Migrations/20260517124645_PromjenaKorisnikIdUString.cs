using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterTrips___Turistička_Agencija.Data.Migrations
{
    /// <inheritdoc />
    public partial class PromjenaKorisnikIdUString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rezervacija_Korisnik_KorisnikId",
                table: "Rezervacija");

            migrationBuilder.DropIndex(
                name: "IX_Rezervacija_KorisnikId",
                table: "Rezervacija");

            migrationBuilder.AlterColumn<string>(
                name: "KorisnikId",
                table: "Rezervacija",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "KorisnikId1",
                table: "Rezervacija",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacija_KorisnikId1",
                table: "Rezervacija",
                column: "KorisnikId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Rezervacija_Korisnik_KorisnikId1",
                table: "Rezervacija",
                column: "KorisnikId1",
                principalTable: "Korisnik",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rezervacija_Korisnik_KorisnikId1",
                table: "Rezervacija");

            migrationBuilder.DropIndex(
                name: "IX_Rezervacija_KorisnikId1",
                table: "Rezervacija");

            migrationBuilder.DropColumn(
                name: "KorisnikId1",
                table: "Rezervacija");

            migrationBuilder.AlterColumn<int>(
                name: "KorisnikId",
                table: "Rezervacija",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacija_KorisnikId",
                table: "Rezervacija",
                column: "KorisnikId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rezervacija_Korisnik_KorisnikId",
                table: "Rezervacija",
                column: "KorisnikId",
                principalTable: "Korisnik",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
