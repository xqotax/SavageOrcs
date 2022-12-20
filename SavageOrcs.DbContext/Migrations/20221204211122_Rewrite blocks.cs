using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SavageOrcs.DbContext.Migrations
{
    public partial class Rewriteblocks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalParameter",
                table: "Blocks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomId",
                table: "Blocks",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalParameter",
                table: "Blocks");

            migrationBuilder.DropColumn(
                name: "CustomId",
                table: "Blocks");
        }
    }
}
