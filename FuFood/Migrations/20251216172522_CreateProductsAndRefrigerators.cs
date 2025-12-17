using System;
using FuFood.Models.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuFood.Migrations
{
    /// <inheritdoc />
    public partial class CreateProductsAndRefrigerators : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:product_unit", "件,個,公克,公升,公斤,包,塊,壺,把,杯,桶,條,毫克,毫升,片,瓶,盒,箱,粒,罐,袋,袋裝,顆");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    Unit = table.Column<UnitType>(type: "product_unit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Refrigerators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Colour = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refrigerators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Refrigerators_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Refrigerators_CreatedById",
                table: "Refrigerators",
                column: "CreatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Refrigerators");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:product_unit", "件,個,公克,公升,公斤,包,塊,壺,把,杯,桶,條,毫克,毫升,片,瓶,盒,箱,粒,罐,袋,袋裝,顆");
        }
    }
}
