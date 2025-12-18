using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuFood.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintForDefaultRefrigerator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Refrigerators_CreatedById",
                table: "Refrigerators");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Refrigerators",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Refrigerators_CreatedById",
                table: "Refrigerators",
                column: "CreatedById",
                unique: true,
                filter: "\"IsDefault\" = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Refrigerators_CreatedById",
                table: "Refrigerators");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Refrigerators");

            migrationBuilder.CreateIndex(
                name: "IX_Refrigerators_CreatedById",
                table: "Refrigerators",
                column: "CreatedById");
        }
    }
}
