using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterTrips___Turistička_Agencija.Data.Migrations
{
    /// <inheritdoc />
    public partial class Izvjestaj : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StatusSlanja",
                table: "LogNotifikacija",
                newName: "TipNotifikacije");

            migrationBuilder.RenameColumn(
                name: "PrimalacEmail",
                table: "LogNotifikacija",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "PokusajBroj",
                table: "LogNotifikacija",
                newName: "BrojPokusaja");

            migrationBuilder.RenameColumn(
                name: "Naslov",
                table: "LogNotifikacija",
                newName: "PorukaGreske");

            migrationBuilder.RenameColumn(
                name: "DetaljiGreske",
                table: "LogNotifikacija",
                newName: "EmailPrimaoca");

            migrationBuilder.RenameColumn(
                name: "DatumSlanja",
                table: "LogNotifikacija",
                newName: "VrijemeSlanja");

            migrationBuilder.AddColumn<string>(
                name: "TransakcijskiKod",
                table: "Placanje",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BrojPregleda",
                table: "Paket",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Ocjena",
                table: "Paket",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "RezervacijaId",
                table: "LogNotifikacija",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1b63ef27-996b-4b13-98db-00f7e4b9bc10",
                column: "ConcurrencyStamp",
                value: "f0faa4ec-9434-4998-8b5c-71a92ebcf746");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c74fa38-885b-3b12-87cb-11e8e5c8cd21",
                column: "ConcurrencyStamp",
                value: "7f035f40-6da2-4c87-80ce-e10fb4290ab0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3d85fb49-774b-2b11-76da-22f9e6d9de32",
                column: "ConcurrencyStamp",
                value: "8e4c2ecd-b827-4707-a066-816b6c499db6");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransakcijskiKod",
                table: "Placanje");

            migrationBuilder.DropColumn(
                name: "BrojPregleda",
                table: "Paket");

            migrationBuilder.DropColumn(
                name: "Ocjena",
                table: "Paket");

            migrationBuilder.DropColumn(
                name: "RezervacijaId",
                table: "LogNotifikacija");

            migrationBuilder.RenameColumn(
                name: "VrijemeSlanja",
                table: "LogNotifikacija",
                newName: "DatumSlanja");

            migrationBuilder.RenameColumn(
                name: "TipNotifikacije",
                table: "LogNotifikacija",
                newName: "StatusSlanja");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "LogNotifikacija",
                newName: "PrimalacEmail");

            migrationBuilder.RenameColumn(
                name: "PorukaGreske",
                table: "LogNotifikacija",
                newName: "Naslov");

            migrationBuilder.RenameColumn(
                name: "EmailPrimaoca",
                table: "LogNotifikacija",
                newName: "DetaljiGreske");

            migrationBuilder.RenameColumn(
                name: "BrojPokusaja",
                table: "LogNotifikacija",
                newName: "PokusajBroj");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1b63ef27-996b-4b13-98db-00f7e4b9bc10",
                column: "ConcurrencyStamp",
                value: "9f86c3d5-8cc7-4692-ac70-3e28e7149710");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c74fa38-885b-3b12-87cb-11e8e5c8cd21",
                column: "ConcurrencyStamp",
                value: "9be574d4-0bef-47df-bfb5-8fd7208666f6");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3d85fb49-774b-2b11-76da-22f9e6d9de32",
                column: "ConcurrencyStamp",
                value: "fd43cefc-b12f-4ffa-848f-0d71db4ba068");
        }
    }
}
