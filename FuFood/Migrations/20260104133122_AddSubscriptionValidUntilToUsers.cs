using System;
using FuFood.Models.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuFood.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionValidUntilToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscriptionTier",
                table: "Users");

            migrationBuilder.AddColumn<DateTime>(
                name: "SubscriptionValidUntil",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscriptionValidUntil",
                table: "Users");

            migrationBuilder.AddColumn<SubscriptionTier>(
                name: "SubscriptionTier",
                table: "Users",
                type: "subscription_tier",
                nullable: false,
                defaultValue: SubscriptionTier.Free);
        }
    }
}
