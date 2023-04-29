using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SavageOrcs.DbContext.Migrations
{
    public partial class Removeplaces : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Areas_AreaTypes_AreaTypeId",
                table: "Areas");

            migrationBuilder.DropTable(
                name: "AreaTypes");

            migrationBuilder.DropTable(
                name: "PlaceToClusters");

            migrationBuilder.DropTable(
                name: "PlaceToMarks");

            migrationBuilder.DropTable(
                name: "Places");

            migrationBuilder.DropIndex(
                name: "IX_Areas_AreaTypeId",
                table: "Areas");

            migrationBuilder.Sql(@"

update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Zakarpattia oblast' where Region = 'ЗАКАРПАТСЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Cherkasy oblast' where Region = 'ЧЕРКАСЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Chernihiv oblast' where Region = 'ЧЕРНІГІВСЬКА ОБЛАСТЬ' --
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Kyiv oblast' where Region = 'КИЇВСЬКА ОБЛАСТЬ' --
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Zhytomyt oblast' where Region = 'ЖИТОМИРСЬКА ОБЛАСТЬ' --
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Chernivtsi oblast' where Region = 'ЧЕРНІВЕЦЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Kherson oblast' where Region = 'ХЕРСОНСЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Autonomous repoblic of crimea' where Region = 'АВТОНОМНА РЕСПУБЛІКА КРИМ' --
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Vinnytsk oblast' where Region = 'ВІННИЦЬКА ОБЛАСТЬ' --
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Ternopil oblast' where Region = 'ТЕРНОПІЛЬСЬКА ОБЛАСТЬ' --
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Ivano-Frankivsk oblast' where Region = 'ІВАНО-ФРАНКІВСЬКА ОБЛАСТЬ' --
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Sumy oblast' where Region = 'СУМСЬКА ОБЛАСТЬ' --
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Poltava oblast' where Region = 'ПОЛТАВСЬКА ОБЛАСТЬ' --
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Mykolaiv oblast' where Region = 'МИКОЛАЇВСЬКА ОБЛАСТЬ' --
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Luhansk oblast' where Region = 'ЛУГАНСЬКА ОБЛАСТЬ' --
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Dnipro oblast' where Region = 'ДНІПРОПЕТРОВСЬКА ОБЛАСТЬ' --

update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Kropyvnytskyi oblast' where Region = 'КІРОВОГРАДСЬКА ОБЛАСТЬ' --
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Donetsk oblast' where Region = 'ДОНЕЦЬКА ОБЛАСТЬ' --
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Khmelnytskyi oblast' where Region = 'ХМЕЛЬНИЦЬКА ОБЛАСТЬ' -- 
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Rivne oblast' where Region = 'РІВНЕНСЬКА ОБЛАСТЬ' --
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Kharkiv oblast' where Region = 'ХАРКІВСЬКА ОБЛАСТЬ' --
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Odesa oblast' where Region = 'ОДЕСЬКА ОБЛАСТЬ' --
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Zaporizhhia oblast' where Region = 'ЗАПОРІЗЬКА ОБЛАСТЬ' --
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Volyn oblast' where Region = 'ВОЛИНСЬКА ОБЛАСТЬ' --
update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Lviv oblast' where Region = 'ЛЬВІВСЬКА ОБЛАСТЬ' --

update [SavageOrcs].[dbo].[Areas] set RegionEng = 'Sevastopol city' where Region = 'М.СЕВАСТОПОЛЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'М. Севастополь' where Region = 'М.СЕВАСТОПОЛЬ'


update [SavageOrcs].[dbo].[Areas] set Region = 'Кіровоградська область' where Region = 'КІРОВОГРАДСЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'Дніпропетровська область' where Region = 'ДНІПРОПЕТРОВСЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'Донецька область' where Region = 'ДОНЕЦЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'Харківська область' where Region = 'ХАРКІВСЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'Луганська область' where Region = 'ЛУГАНСЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'Чернігівська область' where Region = 'ЧЕРНІГІВСЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'Житомирська область' where Region = 'ЖИТОМИРСЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'Київська область' where Region = 'КИЇВСЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'Запорізька область' where Region = 'ЗАПОРІЗЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'Одеська область' where Region = 'ОДЕСЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'Миколаївська область' where Region = 'МИКОЛАЇВСЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'Львівська область' where Region = 'ЛЬВІВСЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'Волинська область' where Region = 'ВОЛИНСЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'рівненська область' where Region = 'РІВНЕНСЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'Хмельницька область' where Region = 'ХМЕЛЬНИЦЬКА ОБЛАСТЬ'

update [SavageOrcs].[dbo].[Areas] set Region = 'Автономна республіка Крим' where Region = 'АВТОНОМНА РЕСПУБЛІКА КРИМ'
update [SavageOrcs].[dbo].[Areas] set Region = 'Вінницька область' where Region = 'ВІННИЦЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'Тернопільскьа область' where Region = 'ТЕРНОПІЛЬСЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'Івано-Франківська область' where Region = 'ІВАНО-ФРАНКІВСЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'Сумська область' where Region = 'СУМСЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'Полтавська область' where Region = 'ПОЛТАВСЬКА ОБЛАСТЬ'

update [SavageOrcs].[dbo].[Areas] set Region = 'Закарпатська область' where Region = 'ЗАКАРПАТСЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'Черкаська область' where Region = 'ЧЕРКАСЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'Чернівецька область' where Region = 'ЧЕРНІВЕЦЬКА ОБЛАСТЬ'
update [SavageOrcs].[dbo].[Areas] set Region = 'Херсонська область' where Region = 'ХЕРСОНСЬКА ОБЛАСТЬ'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AreaTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Places",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameEng = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlaceToClusters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClusterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceToClusters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaceToClusters_Clusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "Clusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaceToClusters_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaceToMarks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceToMarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaceToMarks_Marks_MarkId",
                        column: x => x.MarkId,
                        principalTable: "Marks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaceToMarks_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Areas_AreaTypeId",
                table: "Areas",
                column: "AreaTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceToClusters_ClusterId",
                table: "PlaceToClusters",
                column: "ClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceToClusters_PlaceId",
                table: "PlaceToClusters",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceToMarks_MarkId",
                table: "PlaceToMarks",
                column: "MarkId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceToMarks_PlaceId",
                table: "PlaceToMarks",
                column: "PlaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Areas_AreaTypes_AreaTypeId",
                table: "Areas",
                column: "AreaTypeId",
                principalTable: "AreaTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
