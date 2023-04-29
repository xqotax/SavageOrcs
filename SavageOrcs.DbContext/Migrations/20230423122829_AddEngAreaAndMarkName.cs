using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SavageOrcs.DbContext.Migrations
{
    public partial class AddEngAreaAndMarkName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegisterIsImportant",
                table: "KeyWords");

            migrationBuilder.AddColumn<string>(
                name: "NameEng",
                table: "Marks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEng",
                table: "KeyWords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEng",
                table: "Curators",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEng",
                table: "Clusters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommunityEng",
                table: "Areas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEng",
                table: "Areas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RegionEng",
                table: "Areas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameEng",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "NameEng",
                table: "KeyWords");

            migrationBuilder.DropColumn(
                name: "NameEng",
                table: "Curators");

            migrationBuilder.DropColumn(
                name: "NameEng",
                table: "Clusters");

            migrationBuilder.DropColumn(
                name: "CommunityEng",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "NameEng",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "RegionEng",
                table: "Areas");

            migrationBuilder.AddColumn<bool>(
                name: "RegisterIsImportant",
                table: "KeyWords",
                type: "bit",
                nullable: true);
        }
    }
}
