using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SavageOrcs.DbContext.Migrations
{
    public partial class Mark_Curator_Cluster_Dependency_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResourceName",
                table: "Marks",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResourceName",
                table: "Marks");
        }
    }
}
