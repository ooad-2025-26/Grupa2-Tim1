using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterTrips___Turistička_Agencija.Data.Migrations
{
    /// <inheritdoc />
    public partial class DodanTerminPutovanjaUPakete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TerminPutovanja_Paket_PaketId",
                table: "TerminPutovanja");

            migrationBuilder.AlterColumn<int>(
                name: "PaketId",
                table: "TerminPutovanja",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Aviokompanija",
                table: "Let",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CijenaPoNoci",
                table: "Hoteli",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1b63ef27-996b-4b13-98db-00f7e4b9bc10",
                column: "ConcurrencyStamp",
                value: "0e215a21-d7bc-406a-bdd0-cf49c442d1ef");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c74fa38-885b-3b12-87cb-11e8e5c8cd21",
                column: "ConcurrencyStamp",
                value: "5d10a351-9b12-4742-9c02-38cbb8be9754");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3d85fb49-774b-2b11-76da-22f9e6d9de32",
                column: "ConcurrencyStamp",
                value: "791a9883-a669-4205-9744-c44c96781b27");

            migrationBuilder.AddForeignKey(
                name: "FK_TerminPutovanja_Paket_PaketId",
                table: "TerminPutovanja",
                column: "PaketId",
                principalTable: "Paket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TerminPutovanja_Paket_PaketId",
                table: "TerminPutovanja");

            migrationBuilder.AlterColumn<int>(
                name: "PaketId",
                table: "TerminPutovanja",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Aviokompanija",
                table: "Let",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CijenaPoNoci",
                table: "Hoteli",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1b63ef27-996b-4b13-98db-00f7e4b9bc10",
                column: "ConcurrencyStamp",
                value: "8d61fcbb-c7b0-430b-afc6-c84014e3f680");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c74fa38-885b-3b12-87cb-11e8e5c8cd21",
                column: "ConcurrencyStamp",
                value: "13d6cf11-f36e-4d19-8299-c1c80cd536cf");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3d85fb49-774b-2b11-76da-22f9e6d9de32",
                column: "ConcurrencyStamp",
                value: "2138c9e5-d3c4-4a11-b675-1fa21c72bbcc");

            migrationBuilder.AddForeignKey(
                name: "FK_TerminPutovanja_Paket_PaketId",
                table: "TerminPutovanja",
                column: "PaketId",
                principalTable: "Paket",
                principalColumn: "Id");
        }
    }
}
