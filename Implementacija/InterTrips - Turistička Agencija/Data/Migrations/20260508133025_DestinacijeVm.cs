using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterTrips___Turistička_Agencija.Data.Migrations
{
    /// <inheritdoc />
    public partial class DestinacijeVm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StavkePlana_PlanPutovanja_PlanPutovanjaId",
                table: "StavkePlana");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StavkePlana",
                table: "StavkePlana");

            migrationBuilder.RenameTable(
                name: "StavkePlana",
                newName: "StavkaPlana");

            migrationBuilder.RenameIndex(
                name: "IX_StavkePlana_PlanPutovanjaId",
                table: "StavkaPlana",
                newName: "IX_StavkaPlana_PlanPutovanjaId");

            migrationBuilder.AddColumn<string>(
                name: "Opis",
                table: "Paket",
                type: "nvarchar(800)",
                maxLength: 800,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SlikaUrl",
                table: "Paket",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SlikaUrl",
                table: "Destinacija",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StavkaPlana",
                table: "StavkaPlana",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AgentPaketi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    PaketId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentPaketi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentPaketi_Korisnik_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Korisnik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgentPaketi_Paket_PaketId",
                        column: x => x.PaketId,
                        principalTable: "Paket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgentPaketi_AgentId",
                table: "AgentPaketi",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentPaketi_PaketId",
                table: "AgentPaketi",
                column: "PaketId");

            migrationBuilder.AddForeignKey(
                name: "FK_StavkaPlana_PlanPutovanja_PlanPutovanjaId",
                table: "StavkaPlana",
                column: "PlanPutovanjaId",
                principalTable: "PlanPutovanja",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StavkaPlana_PlanPutovanja_PlanPutovanjaId",
                table: "StavkaPlana");

            migrationBuilder.DropTable(
                name: "AgentPaketi");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StavkaPlana",
                table: "StavkaPlana");

            migrationBuilder.DropColumn(
                name: "Opis",
                table: "Paket");

            migrationBuilder.DropColumn(
                name: "SlikaUrl",
                table: "Paket");

            migrationBuilder.DropColumn(
                name: "SlikaUrl",
                table: "Destinacija");

            migrationBuilder.RenameTable(
                name: "StavkaPlana",
                newName: "StavkePlana");

            migrationBuilder.RenameIndex(
                name: "IX_StavkaPlana_PlanPutovanjaId",
                table: "StavkePlana",
                newName: "IX_StavkePlana_PlanPutovanjaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StavkePlana",
                table: "StavkePlana",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StavkePlana_PlanPutovanja_PlanPutovanjaId",
                table: "StavkePlana",
                column: "PlanPutovanjaId",
                principalTable: "PlanPutovanja",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
