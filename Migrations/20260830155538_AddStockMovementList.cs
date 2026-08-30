using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalStock.Migrations
{
    /// <inheritdoc />
    public partial class AddStockMovementList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockMovement_Batches_BatchId",
                table: "StockMovement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StockMovement",
                table: "StockMovement");

            migrationBuilder.RenameTable(
                name: "StockMovement",
                newName: "StockMovements");

            migrationBuilder.RenameIndex(
                name: "IX_StockMovement_BatchId",
                table: "StockMovements",
                newName: "IX_StockMovements_BatchId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockMovements",
                table: "StockMovements",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Batches_BatchId",
                table: "StockMovements",
                column: "BatchId",
                principalTable: "Batches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Batches_BatchId",
                table: "StockMovements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StockMovements",
                table: "StockMovements");

            migrationBuilder.RenameTable(
                name: "StockMovements",
                newName: "StockMovement");

            migrationBuilder.RenameIndex(
                name: "IX_StockMovements_BatchId",
                table: "StockMovement",
                newName: "IX_StockMovement_BatchId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockMovement",
                table: "StockMovement",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovement_Batches_BatchId",
                table: "StockMovement",
                column: "BatchId",
                principalTable: "Batches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
