using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuFood.Migrations
{
    /// <inheritdoc />
    public partial class RenameFinalizedAtToCommittedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FinalizedAt",
                table: "InventoryTransactions",
                newName: "CommittedAt");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryTransactions_FinalizedAt",
                table: "InventoryTransactions",
                newName: "IX_InventoryTransactions_CommittedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CommittedAt",
                table: "InventoryTransactions",
                newName: "FinalizedAt");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryTransactions_CommittedAt",
                table: "InventoryTransactions",
                newName: "IX_InventoryTransactions_FinalizedAt");
        }
    }
}
