using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SavageOrcs.DbContext.Migrations
{
    public partial class Mark_Lng_Lat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Lng",
                table: "Marks",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Lat",
                table: "Marks",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.Sql(@"  INSERT INTO [dbo].[Areas] (Id, Lvl_1, Lvl_2, Lvl_3, Lvl_4, AreaTypeId, Name, Code, Region, Community)
  VALUES ('6971439C-7C06-401D-BE1F-806C7CBD2BE0', 3200000000, NULL, NULL, NULL, 1, 'КИЇВ', 3200000001, 'КИЇВСЬКА ОБЛАСТЬ', 'КИЇВ'), 
 ('780FE60E-674C-4429-A0F3-3A7EDA72FABE', 6300000000, NULL, NULL, NULL, 1, 'ХАРКІВ', 6300000001, 'ХАРКІВСЬКА ОБЛАСТЬ', 'ХАРКІВ'),
 ('CC0346C4-205C-4028-8FC8-F53733A9D81A', 6500000000, NULL, NULL, NULL, 1, 'ХЕРСОН', 6500000001, 'ХЕРСОНСЬКА ОБЛАСТЬ', 'ХЕРСОН'),
 ('1EF98F63-24C0-4944-B08A-EAAD382476E2', 4800000000, NULL, NULL, NULL, 1, 'МИКОЛАЇВ', 4800000001, 'МИКОЛАЇВСЬКА ОБЛАСТЬ', 'МИКОЛАЇВ'),
 ('32D9A78E-F50B-4807-B918-E5CB79A19993', 7400000000, NULL, NULL, NULL, 1, 'ЧЕРНІГІВ', 7400000001, 'ЧЕРНІГІВСЬКА ОБЛАСТЬ', 'ЧЕРНІГІВ'),
 ('4642CD63-5488-4F89-A448-C69946E2F919', 5900000000, NULL, NULL, NULL, 1, 'СУМИ', 5900000001, 'СУМСЬКА ОБЛАСТЬ', 'СУМИ'),
 ('44CF1E74-7C28-478D-AC21-E04123211896', 1400000000, NULL, NULL, NULL, 1, 'ДОНЕЦЬК', 1400000001, 'ДОНЕЦЬКА ОБЛАСТЬ', 'ДОНЕЦЬК'),
 ('C306C34C-CAA1-45C1-8B4E-EE96B8D70E0E', 1400000000, 1421700000, NULL, NULL, 1, 'МАРІУПОЛЬ', 1421700001, 'ДОНЕЦЬКА ОБЛАСТЬ', 'МАРІУПОЛЬ'),
 ('F0849F85-47E5-47FF-AE04-33C4618DEA1C', 4400000000, NULL, NULL, NULL, 1, 'ЛУГАНСЬК', 4400000001, 'ЛУГАНСЬКА ОБЛАСТЬ', 'ЛУГАНСЬК'),
 ('47B207FF-99F4-4B2F-A331-7D83EDB9FA2A', 2300000000, NULL, NULL, NULL, 1, 'ЗАПОРІЖЖЯ', 2300000001, 'ЗАПОРІЗЬКА ОБЛАСТЬ', 'ЗАПОРІЖЖЯ'),
 ('204BFE6B-F2E2-4362-9848-74CA77373851', 1200000000, NULL, NULL, NULL, 1, 'ДНІПРО', 1200000001, 'ДНІПРОПЕТРОВСЬКА ОБЛАСТЬ', 'ДНІПРО')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Lng",
                table: "Marks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "Lat",
                table: "Marks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
