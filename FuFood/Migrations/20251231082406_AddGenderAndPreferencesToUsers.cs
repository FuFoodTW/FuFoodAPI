using System.Collections.Generic;
using FuFood.Models.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuFood.Migrations
{
    /// <inheritdoc />
    public partial class AddGenderAndPreferencesToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:gender", "不透露,其他,女孩兒,無性別,男孩紙")
                .Annotation("Npgsql:Enum:product_unit", "件,個,公克,公升,公斤,包,塊,壺,把,杯,桶,條,毫克,毫升,片,瓶,盒,箱,粒,罐,袋,袋裝,顆")
                .Annotation("Npgsql:Enum:subscription_tier", "free,pro")
                .OldAnnotation("Npgsql:Enum:product_unit", "件,個,公克,公升,公斤,包,塊,壺,把,杯,桶,條,毫克,毫升,片,瓶,盒,箱,粒,罐,袋,袋裝,顆")
                .OldAnnotation("Npgsql:Enum:subscription_tier", "free,pro");

            migrationBuilder.AlterColumn<string>(
                name: "ProfilePictureUrl",
                table: "Users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomGender",
                table: "Users",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Gender>(
                name: "Gender",
                table: "Users",
                type: "gender",
                nullable: false,
                defaultValue: Gender.NotSpecified);

            migrationBuilder.AddColumn<List<string>>(
                name: "Preferences",
                table: "Users",
                type: "text[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomGender",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Preferences",
                table: "Users");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:product_unit", "件,個,公克,公升,公斤,包,塊,壺,把,杯,桶,條,毫克,毫升,片,瓶,盒,箱,粒,罐,袋,袋裝,顆")
                .Annotation("Npgsql:Enum:subscription_tier", "free,pro")
                .OldAnnotation("Npgsql:Enum:gender", "不透露,其他,女孩兒,無性別,男孩紙")
                .OldAnnotation("Npgsql:Enum:product_unit", "件,個,公克,公升,公斤,包,塊,壺,把,杯,桶,條,毫克,毫升,片,瓶,盒,箱,粒,罐,袋,袋裝,顆")
                .OldAnnotation("Npgsql:Enum:subscription_tier", "free,pro");

            migrationBuilder.AlterColumn<string>(
                name: "ProfilePictureUrl",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
