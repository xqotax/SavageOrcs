using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SavageOrcs.DbContext.Migrations
{
    public partial class Clusters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Curator_AspNetUsers_UserId",
                table: "Curator");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Curator",
                table: "Curator");

            migrationBuilder.RenameTable(
                name: "Curator",
                newName: "Curators");

            migrationBuilder.RenameIndex(
                name: "IX_Curator_UserId",
                table: "Curators",
                newName: "IX_Curators_UserId");

            migrationBuilder.AlterColumn<double>(
                name: "Lng",
                table: "Marks",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "Lat",
                table: "Marks",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<Guid>(
                name: "ClusterId",
                table: "Marks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproximate",
                table: "Marks",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Maps",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Curators",
                table: "Curators",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Clusters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lng = table.Column<double>(type: "float", nullable: false),
                    Lat = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MapId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clusters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clusters_Maps_MapId",
                        column: x => x.MapId,
                        principalTable: "Maps",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Marks_ClusterId",
                table: "Marks",
                column: "ClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_Clusters_MapId",
                table: "Clusters",
                column: "MapId");

            migrationBuilder.AddForeignKey(
                name: "FK_Curators_AspNetUsers_UserId",
                table: "Curators",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Marks_Clusters_ClusterId",
                table: "Marks",
                column: "ClusterId",
                principalTable: "Clusters",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Curators_AspNetUsers_UserId",
                table: "Curators");

            migrationBuilder.DropForeignKey(
                name: "FK_Marks_Clusters_ClusterId",
                table: "Marks");

            migrationBuilder.DropTable(
                name: "Clusters");

            migrationBuilder.DropIndex(
                name: "IX_Marks_ClusterId",
                table: "Marks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Curators",
                table: "Curators");

            migrationBuilder.DropColumn(
                name: "ClusterId",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "IsApproximate",
                table: "Marks");

            migrationBuilder.RenameTable(
                name: "Curators",
                newName: "Curator");

            migrationBuilder.RenameIndex(
                name: "IX_Curators_UserId",
                table: "Curator",
                newName: "IX_Curator_UserId");

            migrationBuilder.AlterColumn<double>(
                name: "Lng",
                table: "Marks",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Lat",
                table: "Marks",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Maps",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Curator",
                table: "Curator",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Curator_AspNetUsers_UserId",
                table: "Curator",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
