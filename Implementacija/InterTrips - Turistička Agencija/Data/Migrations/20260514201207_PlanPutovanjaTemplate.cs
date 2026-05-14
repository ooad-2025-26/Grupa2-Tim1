using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterTrips___Turistička_Agencija.Data.Migrations
{
    /// <inheritdoc />
    public partial class PlanPutovanjaTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlanoviPutovanjaTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaketId = table.Column<int>(type: "int", nullable: false),
                    Napomena = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanoviPutovanjaTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanoviPutovanjaTemplate_Paket_PaketId",
                        column: x => x.PaketId,
                        principalTable: "Paket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StavkePlanaTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanPutovanjaTemplateId = table.Column<int>(type: "int", nullable: false),
                    RedniBroj = table.Column<int>(type: "int", nullable: false),
                    Naziv = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    Vrijeme = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StavkePlanaTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StavkePlanaTemplate_PlanoviPutovanjaTemplate_PlanPutovanjaTemplateId",
                        column: x => x.PlanPutovanjaTemplateId,
                        principalTable: "PlanoviPutovanjaTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanoviPutovanjaTemplate_PaketId",
                table: "PlanoviPutovanjaTemplate",
                column: "PaketId");

            migrationBuilder.CreateIndex(
                name: "IX_StavkePlanaTemplate_PlanPutovanjaTemplateId",
                table: "StavkePlanaTemplate",
                column: "PlanPutovanjaTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StavkePlanaTemplate");

            migrationBuilder.DropTable(
                name: "PlanoviPutovanjaTemplate");
        }
    }
}
