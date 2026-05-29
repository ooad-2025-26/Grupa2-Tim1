using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterTrips___Turistička_Agencija.Data.Migrations
{
    /// <inheritdoc />
    public partial class DodajKontaktUpite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KontaktUpit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tema = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Poruka = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumSlanja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Procitano = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KontaktUpit", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1b63ef27-996b-4b13-98db-00f7e4b9bc10",
                column: "ConcurrencyStamp",
                value: "49ebe3ab-44f1-4aef-b5c9-3c3ab2a7da16");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c74fa38-885b-3b12-87cb-11e8e5c8cd21",
                column: "ConcurrencyStamp",
                value: "8f63f2c9-4763-41de-8157-b5b7a3792995");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3d85fb49-774b-2b11-76da-22f9e6d9de32",
                column: "ConcurrencyStamp",
                value: "40eecdce-2fb6-481e-8df1-6b6d1b1a3156");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KontaktUpit");

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
        }
    }
}
