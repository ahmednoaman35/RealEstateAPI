using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateAPI.API.Migrations
{
    /// <inheritdoc />
    public partial class user : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        UPDATE Users
        SET Role =
            CASE
                WHEN Role = 'User' THEN '0'
                WHEN Role = 'Admin' THEN '1'
                ELSE '0'
            END
    ");

            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.Sql(@"
        UPDATE Users
        SET Role =
            CASE
                WHEN Role = '0' THEN 'User'
                WHEN Role = '1' THEN 'Admin'
                ELSE 'User'
            END
    ");
        }
    }
}
