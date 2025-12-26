using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuFood.Migrations
{
    /// <inheritdoc />
    public partial class AddFullyConsumedAtToInventoryTransactionItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "ExpirationDate",
                table: "InventoryTransactionsItems",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<DateTime>(
                name: "FullyConsumedAt",
                table: "InventoryTransactionsItems",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactionsItems_FullyConsumedAt",
                table: "InventoryTransactionsItems",
                column: "FullyConsumedAt");

            migrationBuilder.AddCheckConstraint(
                name: "item_expiration_date_check",
                table: "InventoryTransactionsItems",
                sql: "\"ParentId\" is not null or \"ExpirationDate\" is not null");

            migrationBuilder.AddCheckConstraint(
                name: "item_quantity_sign_check",
                table: "InventoryTransactionsItems",
                sql: "(\"ParentId\" is null and \"Quantity\" > 0) or (\"ParentId\" is not null and \"Quantity\" < 0)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactionsItems_FullyConsumedAt",
                table: "InventoryTransactionsItems");

            migrationBuilder.DropCheckConstraint(
                name: "item_expiration_date_check",
                table: "InventoryTransactionsItems");

            migrationBuilder.DropCheckConstraint(
                name: "item_quantity_sign_check",
                table: "InventoryTransactionsItems");

            migrationBuilder.DropColumn(
                name: "FullyConsumedAt",
                table: "InventoryTransactionsItems");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ExpirationDate",
                table: "InventoryTransactionsItems",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
