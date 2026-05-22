using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterTrips___Turistička_Agencija.Data.Migrations
{
    /// <inheritdoc />
    public partial class DodanKupon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HotelId",
                table: "Paket",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LetId",
                table: "Paket",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1b63ef27-996b-4b13-98db-00f7e4b9bc10",
                column: "ConcurrencyStamp",
                value: "84751eb0-851b-4e08-88a8-0ec2052d1e79");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c74fa38-885b-3b12-87cb-11e8e5c8cd21",
                column: "ConcurrencyStamp",
                value: "152d550f-e9da-4a09-8041-103aa1ae0921");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3d85fb49-774b-2b11-76da-22f9e6d9de32",
                column: "ConcurrencyStamp",
                value: "5590df4c-ec14-489e-a4a9-d436a2130790");

            migrationBuilder.CreateIndex(
                name: "IX_Paket_HotelId",
                table: "Paket",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Paket_LetId",
                table: "Paket",
                column: "LetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Paket_Hoteli_HotelId",
                table: "Paket",
                column: "HotelId",
                principalTable: "Hoteli",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Paket_Let_LetId",
                table: "Paket",
                column: "LetId",
                principalTable: "Let",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Paket_Hoteli_HotelId",
                table: "Paket");

            migrationBuilder.DropForeignKey(
                name: "FK_Paket_Let_LetId",
                table: "Paket");

            migrationBuilder.DropIndex(
                name: "IX_Paket_HotelId",
                table: "Paket");

            migrationBuilder.DropIndex(
                name: "IX_Paket_LetId",
                table: "Paket");

            migrationBuilder.DropColumn(
                name: "HotelId",
                table: "Paket");

            migrationBuilder.DropColumn(
                name: "LetId",
                table: "Paket");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1b63ef27-996b-4b13-98db-00f7e4b9bc10",
                column: "ConcurrencyStamp",
                value: "5af0dcd3-ce47-4730-b820-1955b56f308d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c74fa38-885b-3b12-87cb-11e8e5c8cd21",
                column: "ConcurrencyStamp",
                value: "46910c3c-276a-497f-b4e0-ed2889786f66");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3d85fb49-774b-2b11-76da-22f9e6d9de32",
                column: "ConcurrencyStamp",
                value: "93dac7d8-4081-4737-b742-26a7d60b9408");
        }
    }
}
