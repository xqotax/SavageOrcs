using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SavageOrcs.DbContext.Migrations
{
    public partial class Urlsintext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TextsToClusters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClusterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TextId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextsToClusters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TextsToClusters_Clusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "Clusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TextsToClusters_Texts_TextId",
                        column: x => x.TextId,
                        principalTable: "Texts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TextsToMarks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TextId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextsToMarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TextsToMarks_Marks_MarkId",
                        column: x => x.MarkId,
                        principalTable: "Marks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TextsToMarks_Texts_TextId",
                        column: x => x.TextId,
                        principalTable: "Texts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TextsToClusters_ClusterId",
                table: "TextsToClusters",
                column: "ClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_TextsToClusters_TextId",
                table: "TextsToClusters",
                column: "TextId");

            migrationBuilder.CreateIndex(
                name: "IX_TextsToMarks_MarkId",
                table: "TextsToMarks",
                column: "MarkId");

            migrationBuilder.CreateIndex(
                name: "IX_TextsToMarks_TextId",
                table: "TextsToMarks",
                column: "TextId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TextsToClusters");

            migrationBuilder.DropTable(
                name: "TextsToMarks");
        }
    }
}
