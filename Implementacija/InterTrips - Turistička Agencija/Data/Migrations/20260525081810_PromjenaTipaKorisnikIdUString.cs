using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterTrips___Turistička_Agencija.Data.Migrations
{
    /// <inheritdoc />
    public partial class PromjenaTipaKorisnikIdUString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rezervacija_Korisnik_KorisnikId",
                table: "Rezervacija");

            migrationBuilder.AlterColumn<string>(
                name: "KorisnikId",
                table: "Rezervacija",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Rezervacija_AspNetUsers_KorisnikId",
                table: "Rezervacija",
                column: "KorisnikId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1b63ef27-996b-4b13-98db-00f7e4b9bc10",
                column: "ConcurrencyStamp",
                value: "19d056f6-b5ee-46f6-8ad0-7147dfd771d8");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c74fa38-885b-3b12-87cb-11e8e5c8cd21",
                column: "ConcurrencyStamp",
                value: "408f14a9-9eae-484f-8a63-c92faf680409");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3d85fb49-774b-2b11-76da-22f9e6d9de32",
                column: "ConcurrencyStamp",
                value: "38a8f7af-3ff9-4822-895b-71c453837dc9");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rezervacija_AspNetUsers_KorisnikId",
                table: "Rezervacija");

            migrationBuilder.AlterColumn<int>(
                name: "KorisnikId",
                table: "Rezervacija",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Rezervacija_Korisnik_KorisnikId",
                table: "Rezervacija",
                column: "KorisnikId",
                principalTable: "Korisnik",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1b63ef27-996b-4b13-98db-00f7e4b9bc10",
                column: "ConcurrencyStamp",
                value: "3b260f01-b3f1-42a8-968b-13caad7787ff");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c74fa38-885b-3b12-87cb-11e8e5c8cd21",
                column: "ConcurrencyStamp",
                value: "79d96f9a-36e2-4a86-8fd3-7d4badcedc36");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3d85fb49-774b-2b11-76da-22f9e6d9de32",
                column: "ConcurrencyStamp",
                value: "66630e3e-75fe-4f95-9389-5112c743b830");
        }
    }
}