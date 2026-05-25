using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterTrips___Turistička_Agencija.Data.Migrations
{
    /// <inheritdoc />
    public partial class DodajProcitanaULogNotifikacija : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DatumProcitano",
                table: "LogNotifikacija",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Procitana",
                table: "LogNotifikacija",
                type: "bit",
                nullable: false,
                defaultValue: false);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DatumProcitano",
                table: "LogNotifikacija");

            migrationBuilder.DropColumn(
                name: "Procitana",
                table: "LogNotifikacija");

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
    }
}
