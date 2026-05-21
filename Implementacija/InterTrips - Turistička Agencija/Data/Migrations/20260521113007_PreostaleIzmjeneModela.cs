using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterTrips___Turistička_Agencija.Data.Migrations
{
    /// <inheritdoc />
    public partial class PreostaleIzmjeneModela : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgentPaketi_Korisnik_AgentId",
                table: "AgentPaketi");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentPaketi_Paket_PaketId",
                table: "AgentPaketi");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AgentPaketi",
                table: "AgentPaketi");

            migrationBuilder.RenameTable(
                name: "AgentPaketi",
                newName: "AgentPaket");

            migrationBuilder.RenameIndex(
                name: "IX_AgentPaketi_PaketId",
                table: "AgentPaket",
                newName: "IX_AgentPaket_PaketId");

            migrationBuilder.RenameIndex(
                name: "IX_AgentPaketi_AgentId",
                table: "AgentPaket",
                newName: "IX_AgentPaket_AgentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AgentPaket",
                table: "AgentPaket",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AgentPaket_Korisnik_AgentId",
                table: "AgentPaket",
                column: "AgentId",
                principalTable: "Korisnik",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentPaket_Paket_PaketId",
                table: "AgentPaket",
                column: "PaketId",
                principalTable: "Paket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgentPaket_Korisnik_AgentId",
                table: "AgentPaket");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentPaket_Paket_PaketId",
                table: "AgentPaket");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AgentPaket",
                table: "AgentPaket");

            migrationBuilder.RenameTable(
                name: "AgentPaket",
                newName: "AgentPaketi");

            migrationBuilder.RenameIndex(
                name: "IX_AgentPaket_PaketId",
                table: "AgentPaketi",
                newName: "IX_AgentPaketi_PaketId");

            migrationBuilder.RenameIndex(
                name: "IX_AgentPaket_AgentId",
                table: "AgentPaketi",
                newName: "IX_AgentPaketi_AgentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AgentPaketi",
                table: "AgentPaketi",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AgentPaketi_Korisnik_AgentId",
                table: "AgentPaketi",
                column: "AgentId",
                principalTable: "Korisnik",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentPaketi_Paket_PaketId",
                table: "AgentPaketi",
                column: "PaketId",
                principalTable: "Paket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
