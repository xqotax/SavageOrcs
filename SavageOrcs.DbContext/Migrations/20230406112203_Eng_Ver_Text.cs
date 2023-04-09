using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SavageOrcs.DbContext.Migrations
{
    public partial class Eng_Ver_Text : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EnglishVersion",
                table: "Texts",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.Sql(@"UPDATE dbo.Texts set EnglishVersion = 0");

            migrationBuilder.AlterColumn<bool>(
                name: "EnglishVersion",
                table: "Texts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UkrTextId",
                table: "Texts",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnglishVersion",
                table: "Texts");

            migrationBuilder.DropColumn(
                name: "UkrTextId",
                table: "Texts");
        }
    }
}
