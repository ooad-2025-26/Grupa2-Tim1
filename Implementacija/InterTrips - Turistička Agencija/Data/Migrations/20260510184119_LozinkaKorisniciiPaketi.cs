using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterTrips___Turistička_Agencija.Data.Migrations
{
    /// <inheritdoc />
    public partial class LozinkaKorisniciiPaketi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Lozinka",
                table: "Korisnik",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lozinka",
                table: "Korisnik");
        }
    }
}
