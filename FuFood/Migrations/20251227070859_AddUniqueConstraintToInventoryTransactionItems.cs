using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuFood.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToInventoryTransactionItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactionsItems_ParentId",
                table: "InventoryTransactionsItems");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactionsItems_ParentId_InventoryTransactionId",
                table: "InventoryTransactionsItems",
                columns: new[] { "ParentId", "InventoryTransactionId" },
                unique: true,
                filter: "\"ParentId\" is not null");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactionsItems_ParentId_InventoryTransactionId",
                table: "InventoryTransactionsItems");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactionsItems_ParentId",
                table: "InventoryTransactionsItems",
                column: "ParentId");
        }
    }
}
