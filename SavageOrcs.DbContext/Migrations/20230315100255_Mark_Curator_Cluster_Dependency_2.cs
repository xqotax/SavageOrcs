using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SavageOrcs.DbContext.Migrations
{
    public partial class Mark_Curator_Cluster_Dependency_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionEng",
                table: "Clusters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResourceName",
                table: "Clusters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResourceUrl",
                table: "Clusters",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionEng",
                table: "Clusters");

            migrationBuilder.DropColumn(
                name: "ResourceName",
                table: "Clusters");

            migrationBuilder.DropColumn(
                name: "ResourceUrl",
                table: "Clusters");
        }
    }
}
