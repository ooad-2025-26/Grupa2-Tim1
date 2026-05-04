using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterTrips___Turistička_Agencija.Data.Migrations
{
    /// <inheritdoc />
    public partial class UklonjenStavkeIzPlanPutovanja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StavkaPlana_PlanPutovanja_PlanPutovanjaId",
                table: "StavkaPlana");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StavkaPlana",
                table: "StavkaPlana");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddPrimaryKey(
                name: "PK_StavkaPlana",
                table: "StavkaPlana",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StavkaPlana_PlanPutovanja_PlanPutovanjaId",
                table: "StavkaPlana",
                column: "PlanPutovanjaId",
                principalTable: "PlanPutovanja",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
