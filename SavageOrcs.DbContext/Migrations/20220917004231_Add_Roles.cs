using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SavageOrcs.DbContext.Migrations
{
    public partial class Add_Roles : Migration
    {
        private string GlobalAdminRoleId = "00110011-0011-0011-0011-001100110011";
        private string AdminRoleId = "01110111-0111-0111-0111-011101110111";
        private string SimpleUserRoleId = "11111111-1111-1111-1111-111111111111";
        private string MainDeveloperId = "00010001-0001-0001-0001-000100010001";
        private string SecondDeveloperId = "10011001-1001-1001-1001-100110011001";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
            VALUES ('{GlobalAdminRoleId}', 'Global admin', 'GLOBAL ADMIN', null);");

            migrationBuilder.Sql(@$"INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
            VALUES ('{AdminRoleId}', 'Admin', 'ADMIN', null);");

            migrationBuilder.Sql(@$"INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
            VALUES ('{SimpleUserRoleId}', 'Simple user', 'SIMPLE USER', null);");

            migrationBuilder.Sql(@$"INSERT INTO [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [FromUkraine], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], 
            [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount])
            VALUES (N'{SecondDeveloperId}', N'Юрій', N'Локальний адмін', 1, N'SKRIPNIK.PETRO@KNU.UA', N'SKRIPNIK.PETRO@KNU.UA', N'skripnik.petro@knu.ua', N'SKRIPNIK.PETRO@knu.ua', 1, 
            N'AQAAAAEAACcQAAAAENZk182+PTgb1TX5BKL/inF4iwXBCZuWvyfxIohQ1tK65wqhiN+TWnqEOKWBQsN9kA==', 
            N'5DICKXQUV5EHSUJBA4VC6DJDFXDZGNY6', N'e6093414-b95d-425a-8c31-f1032c8f46f3', NULL, 0, 0, NULL, 1, 0)");

            migrationBuilder.Sql(@$"INSERT INTO [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [FromUkraine], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], 
            [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount])
            VALUES (N'{MainDeveloperId}', N'Юрій', N'Скрипник', 1, N'skripnik.petro@gmail.com', N'SKRIPNIK.PETRO@GMAIL.COM', N'skripnik.petro@gmail.com', N'SKRIPNIK.PETRO@GMAIL.COM', 1, 
N'AQAAAAEAACcQAAAAELO+yxwIu66dh8WvFOFJemczXMpB5R8bSUqsgH2e++EN66sjAjfIOYAhkw5S8nQrYQ==', 
N'D5YEPGHSW6PYCOR47XARDKZWVWWB4SBK', N'1f48a50b-f161-4fa2-8865-5cb332e22d90', NULL, 0, 0, NULL, 1, 0)");

            migrationBuilder.Sql($@"INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId])
            VALUES ('{MainDeveloperId}', '{GlobalAdminRoleId}');
            INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId])
            VALUES ('{MainDeveloperId}', '{AdminRoleId}');
            INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId])
            VALUES ('{MainDeveloperId}', '{SimpleUserRoleId}');");

            migrationBuilder.Sql($@"
            INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId])
            VALUES ('{SecondDeveloperId}', '{AdminRoleId}');
            INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId])
            VALUES ('{SecondDeveloperId}', '{SimpleUserRoleId}');");



        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
