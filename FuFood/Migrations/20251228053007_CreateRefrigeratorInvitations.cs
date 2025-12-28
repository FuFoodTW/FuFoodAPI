using System;
using FuFood.Models.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuFood.Migrations
{
    /// <inheritdoc />
    public partial class CreateRefrigeratorInvitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refrigerators_Users_CreatedById",
                table: "Refrigerators");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Refrigerators",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Refrigerators_CreatedById",
                table: "Refrigerators",
                newName: "IX_Refrigerators_OwnerId");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:product_unit", "件,個,公克,公升,公斤,包,塊,壺,把,杯,桶,條,毫克,毫升,片,瓶,盒,箱,粒,罐,袋,袋裝,顆")
                .Annotation("Npgsql:Enum:subscription_tier", "free,pro")
                .OldAnnotation("Npgsql:Enum:product_unit", "件,個,公克,公升,公斤,包,塊,壺,把,杯,桶,條,毫克,毫升,片,瓶,盒,箱,粒,罐,袋,袋裝,顆");

            migrationBuilder.AddColumn<SubscriptionTier>(
                name: "SubscriptionTier",
                table: "Users",
                type: "subscription_tier",
                nullable: false,
                defaultValue: SubscriptionTier.Free);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Refrigerators",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateTime>(
                name: "NameUpdatedAt",
                table: "Refrigerators",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "RefrigeratorInvitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenHash = table.Column<byte[]>(type: "bytea", nullable: false),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    RefrigeratorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefrigeratorInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefrigeratorInvitations_Refrigerators_RefrigeratorId",
                        column: x => x.RefrigeratorId,
                        principalTable: "Refrigerators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RefrigeratorInvitations_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefrigeratorMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RefrigeratorId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefrigeratorMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefrigeratorMembers_Refrigerators_RefrigeratorId",
                        column: x => x.RefrigeratorId,
                        principalTable: "Refrigerators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RefrigeratorMembers_Users_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefrigeratorInvitations_CreatorId",
                table: "RefrigeratorInvitations",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_RefrigeratorInvitations_RefrigeratorId",
                table: "RefrigeratorInvitations",
                column: "RefrigeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_RefrigeratorMembers_MemberId",
                table: "RefrigeratorMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_RefrigeratorMembers_RefrigeratorId_MemberId",
                table: "RefrigeratorMembers",
                columns: new[] { "RefrigeratorId", "MemberId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Refrigerators_Users_OwnerId",
                table: "Refrigerators",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refrigerators_Users_OwnerId",
                table: "Refrigerators");

            migrationBuilder.DropTable(
                name: "RefrigeratorInvitations");

            migrationBuilder.DropTable(
                name: "RefrigeratorMembers");

            migrationBuilder.DropColumn(
                name: "SubscriptionTier",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NameUpdatedAt",
                table: "Refrigerators");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Refrigerators",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Refrigerators_OwnerId",
                table: "Refrigerators",
                newName: "IX_Refrigerators_CreatedById");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:product_unit", "件,個,公克,公升,公斤,包,塊,壺,把,杯,桶,條,毫克,毫升,片,瓶,盒,箱,粒,罐,袋,袋裝,顆")
                .OldAnnotation("Npgsql:Enum:product_unit", "件,個,公克,公升,公斤,包,塊,壺,把,杯,桶,條,毫克,毫升,片,瓶,盒,箱,粒,罐,袋,袋裝,顆")
                .OldAnnotation("Npgsql:Enum:subscription_tier", "free,pro");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Refrigerators",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AddForeignKey(
                name: "FK_Refrigerators_Users_CreatedById",
                table: "Refrigerators",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
