using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterTrips___Turistička_Agencija.Data.Migrations
{
    /// <inheritdoc />
    public partial class DodanDostupniPrevozUPaket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DatumPolaska",
                table: "Paket",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "DostupniPrevoz",
                table: "Paket",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Odrediste",
                table: "Let",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Polazak",
                table: "Let",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DestinacijaId",
                table: "Hoteli",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1b63ef27-996b-4b13-98db-00f7e4b9bc10",
                column: "ConcurrencyStamp",
                value: "80483b5c-e438-4478-843c-c59ea719c7d6");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c74fa38-885b-3b12-87cb-11e8e5c8cd21",
                column: "ConcurrencyStamp",
                value: "f4b4b701-4799-4a27-a2fd-9ec450c609a2");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3d85fb49-774b-2b11-76da-22f9e6d9de32",
                column: "ConcurrencyStamp",
                value: "9abcb829-c4c8-4027-a828-f4d65e5f188d");

            migrationBuilder.CreateIndex(
                name: "IX_Hoteli_DestinacijaId",
                table: "Hoteli",
                column: "DestinacijaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hoteli_Destinacija_DestinacijaId",
                table: "Hoteli",
                column: "DestinacijaId",
                principalTable: "Destinacija",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hoteli_Destinacija_DestinacijaId",
                table: "Hoteli");

            migrationBuilder.DropIndex(
                name: "IX_Hoteli_DestinacijaId",
                table: "Hoteli");

            migrationBuilder.DropColumn(
                name: "DostupniPrevoz",
                table: "Paket");

            migrationBuilder.DropColumn(
                name: "Odrediste",
                table: "Let");

            migrationBuilder.DropColumn(
                name: "Polazak",
                table: "Let");

            migrationBuilder.DropColumn(
                name: "DestinacijaId",
                table: "Hoteli");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DatumPolaska",
                table: "Paket",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

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
        }
    }
}
