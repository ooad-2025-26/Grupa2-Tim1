using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterTrips___Turistička_Agencija.Data.Migrations
{
    /// <inheritdoc />
    public partial class Kuponi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KuponId",
                table: "Placanje",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalniIznos",
                table: "Placanje",
                type: "decimal(18,2)",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Placanje_KuponId",
                table: "Placanje",
                column: "KuponId");

            migrationBuilder.AddForeignKey(
                name: "FK_Placanje_Kupon_KuponId",
                table: "Placanje",
                column: "KuponId",
                principalTable: "Kupon",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Placanje_Kupon_KuponId",
                table: "Placanje");

            migrationBuilder.DropIndex(
                name: "IX_Placanje_KuponId",
                table: "Placanje");

            migrationBuilder.DropColumn(
                name: "KuponId",
                table: "Placanje");

            migrationBuilder.DropColumn(
                name: "OriginalniIznos",
                table: "Placanje");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1b63ef27-996b-4b13-98db-00f7e4b9bc10",
                column: "ConcurrencyStamp",
                value: "7e1eec60-0735-4b18-b42d-85033e6eb12d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c74fa38-885b-3b12-87cb-11e8e5c8cd21",
                column: "ConcurrencyStamp",
                value: "ba799f48-a20e-4fda-8bce-d77b1a319f17");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3d85fb49-774b-2b11-76da-22f9e6d9de32",
                column: "ConcurrencyStamp",
                value: "52bd4ed9-a5b0-4432-a11a-71bd72395517");
        }
    }
}
