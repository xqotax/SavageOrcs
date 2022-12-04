using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SavageOrcs.DbContext.Migrations
{
    public partial class Edit_Cluster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreationDate",
                table: "Clusters",
                newName: "CreatedDate");

            migrationBuilder.AddColumn<Guid>(
                name: "AreaId",
                table: "Clusters",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clusters_AreaId",
                table: "Clusters",
                column: "AreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clusters_Areas_AreaId",
                table: "Clusters",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clusters_Areas_AreaId",
                table: "Clusters");

            migrationBuilder.DropIndex(
                name: "IX_Clusters_AreaId",
                table: "Clusters");

            migrationBuilder.DropColumn(
                name: "AreaId",
                table: "Clusters");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Clusters",
                newName: "CreationDate");
        }
    }
}
