using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SavageOrcs.DbContext.Migrations
{
    public partial class Mark_Curator_Cluster_Dependency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Curators_AspNetUsers_UserId",
                table: "Curators");

            migrationBuilder.AddColumn<Guid>(
                name: "CuratorId",
                table: "Marks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Curators",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<Guid>(
                name: "CuratorId",
                table: "Clusters",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Marks_CuratorId",
                table: "Marks",
                column: "CuratorId");

            migrationBuilder.CreateIndex(
                name: "IX_Clusters_CuratorId",
                table: "Clusters",
                column: "CuratorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clusters_Curators_CuratorId",
                table: "Clusters",
                column: "CuratorId",
                principalTable: "Curators",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Curators_AspNetUsers_UserId",
                table: "Curators",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Marks_Curators_CuratorId",
                table: "Marks",
                column: "CuratorId",
                principalTable: "Curators",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clusters_Curators_CuratorId",
                table: "Clusters");

            migrationBuilder.DropForeignKey(
                name: "FK_Curators_AspNetUsers_UserId",
                table: "Curators");

            migrationBuilder.DropForeignKey(
                name: "FK_Marks_Curators_CuratorId",
                table: "Marks");

            migrationBuilder.DropIndex(
                name: "IX_Marks_CuratorId",
                table: "Marks");

            migrationBuilder.DropIndex(
                name: "IX_Clusters_CuratorId",
                table: "Clusters");

            migrationBuilder.DropColumn(
                name: "CuratorId",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "CuratorId",
                table: "Clusters");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Curators",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Curators_AspNetUsers_UserId",
                table: "Curators",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
