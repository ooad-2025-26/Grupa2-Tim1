using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterTrips___Turistička_Agencija.Data.Migrations
{
    /// <inheritdoc />
    public partial class SredjenContextPopravljen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hoteli_Destinacija_DestinacijaId",
                table: "Hoteli");

            migrationBuilder.DropForeignKey(
                name: "FK_Paket_Hoteli_HotelId",
                table: "Paket");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Hoteli",
                table: "Hoteli");

            migrationBuilder.RenameTable(
                name: "Hoteli",
                newName: "Hotel");

            migrationBuilder.RenameIndex(
                name: "IX_Hoteli_DestinacijaId",
                table: "Hotel",
                newName: "IX_Hotel_DestinacijaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Hotel",
                table: "Hotel",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1b63ef27-996b-4b13-98db-00f7e4b9bc10",
                column: "ConcurrencyStamp",
                value: "f27730ee-a8e6-4095-a5ca-2488b6ecbcd6");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c74fa38-885b-3b12-87cb-11e8e5c8cd21",
                column: "ConcurrencyStamp",
                value: "ffc8a999-55cd-4cc8-89a9-24d003c804ee");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3d85fb49-774b-2b11-76da-22f9e6d9de32",
                column: "ConcurrencyStamp",
                value: "3f7f2a22-0f29-4864-9f75-7debed486673");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotel_Destinacija_DestinacijaId",
                table: "Hotel",
                column: "DestinacijaId",
                principalTable: "Destinacija",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Paket_Hotel_HotelId",
                table: "Paket",
                column: "HotelId",
                principalTable: "Hotel",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hotel_Destinacija_DestinacijaId",
                table: "Hotel");

            migrationBuilder.DropForeignKey(
                name: "FK_Paket_Hotel_HotelId",
                table: "Paket");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Hotel",
                table: "Hotel");

            migrationBuilder.RenameTable(
                name: "Hotel",
                newName: "Hoteli");

            migrationBuilder.RenameIndex(
                name: "IX_Hotel_DestinacijaId",
                table: "Hoteli",
                newName: "IX_Hoteli_DestinacijaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Hoteli",
                table: "Hoteli",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1b63ef27-996b-4b13-98db-00f7e4b9bc10",
                column: "ConcurrencyStamp",
                value: "a9f768a3-b855-4c80-ba1b-e841fd35a9ad");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c74fa38-885b-3b12-87cb-11e8e5c8cd21",
                column: "ConcurrencyStamp",
                value: "573b043e-bb96-4ff1-9d5b-92ceb181a30e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3d85fb49-774b-2b11-76da-22f9e6d9de32",
                column: "ConcurrencyStamp",
                value: "fcbe15ab-b66e-4a90-a88f-50209c7d16f9");

            migrationBuilder.AddForeignKey(
                name: "FK_Hoteli_Destinacija_DestinacijaId",
                table: "Hoteli",
                column: "DestinacijaId",
                principalTable: "Destinacija",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Paket_Hoteli_HotelId",
                table: "Paket",
                column: "HotelId",
                principalTable: "Hoteli",
                principalColumn: "Id");
        }
    }
}
