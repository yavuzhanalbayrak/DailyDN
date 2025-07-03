using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyDN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class baseEntityConfiguration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "UserSessions",
                type: "int",
                nullable: false,
                defaultValueSql: "0",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValueSql: "0",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "UserRoles",
                type: "int",
                nullable: false,
                defaultValueSql: "0",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "Roles",
                type: "int",
                nullable: false,
                defaultValueSql: "0",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "RoleClaims",
                type: "int",
                nullable: false,
                defaultValueSql: "0",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "Claims",
                type: "int",
                nullable: false,
                defaultValueSql: "0",
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "UserSessions",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "0");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "0");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "UserRoles",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "0");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "Roles",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "0");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "RoleClaims",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "0");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "Claims",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "0");
        }
    }
}
