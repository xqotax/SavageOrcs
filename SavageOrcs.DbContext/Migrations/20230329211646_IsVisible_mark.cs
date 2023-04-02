using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SavageOrcs.DbContext.Migrations
{
    public partial class IsVisible_mark : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproximate",
                table: "Marks");

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "Marks",
                type: "bit",
                nullable: true,
                defaultValue: false);


            migrationBuilder.Sql(@"Update dbo.Marks set IsVisible = 1");

            migrationBuilder.AlterColumn<bool>(
                name: "IsVisible",
                table: "Marks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "Marks");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproximate",
                table: "Marks",
                type: "bit",
                nullable: true);
        }
    }
}
