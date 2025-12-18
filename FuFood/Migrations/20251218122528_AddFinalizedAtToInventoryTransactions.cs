using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuFood.Migrations
{
    /// <inheritdoc />
    public partial class AddFinalizedAtToInventoryTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FinalizedAt",
                table: "InventoryTransactions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_FinalizedAt",
                table: "InventoryTransactions",
                column: "FinalizedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_FinalizedAt",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "FinalizedAt",
                table: "InventoryTransactions");
        }
    }
}
