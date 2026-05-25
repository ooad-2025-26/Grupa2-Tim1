using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterTrips___Turistička_Agencija.Data.Migrations
{
    /// <inheritdoc />
    public partial class PoveziLogSaRezervacijom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1b63ef27-996b-4b13-98db-00f7e4b9bc10",
                column: "ConcurrencyStamp",
                value: "49930061-127c-4f95-80ef-19b6381ab11a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c74fa38-885b-3b12-87cb-11e8e5c8cd21",
                column: "ConcurrencyStamp",
                value: "a7bca189-63b4-44c7-bc7b-4d26c8741b23");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3d85fb49-774b-2b11-76da-22f9e6d9de32",
                column: "ConcurrencyStamp",
                value: "cfd44737-9fd4-4da0-8930-ca04ce43bc27");

            migrationBuilder.CreateIndex(
                name: "IX_LogNotifikacija_RezervacijaId",
                table: "LogNotifikacija",
                column: "RezervacijaId");

            migrationBuilder.AddForeignKey(
                name: "FK_LogNotifikacija_Rezervacija_RezervacijaId",
                table: "LogNotifikacija",
                column: "RezervacijaId",
                principalTable: "Rezervacija",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogNotifikacija_Rezervacija_RezervacijaId",
                table: "LogNotifikacija");

            migrationBuilder.DropIndex(
                name: "IX_LogNotifikacija_RezervacijaId",
                table: "LogNotifikacija");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1b63ef27-996b-4b13-98db-00f7e4b9bc10",
                column: "ConcurrencyStamp",
                value: "2c03ffa8-de44-4bb1-9d68-547d11eddf10");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c74fa38-885b-3b12-87cb-11e8e5c8cd21",
                column: "ConcurrencyStamp",
                value: "b8b8f545-f19c-4c53-a641-c86956642739");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3d85fb49-774b-2b11-76da-22f9e6d9de32",
                column: "ConcurrencyStamp",
                value: "41e35708-6af2-4e73-a107-84dbb4803f0d");
        }
    }
}
