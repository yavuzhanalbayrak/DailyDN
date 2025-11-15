using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyDN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class userSessionRefTokenHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                table: "UserSessions",
                newName: "RefreshTokenHash");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessions_RefreshToken",
                table: "UserSessions",
                newName: "IX_UserSessions_RefreshTokenHash");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELAUs+nJPSlymbpaEf2On5XTsZilCbc+jpMAqhini8fYQU/yeTEKm1diq/A5/pcfWw==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshTokenHash",
                table: "UserSessions",
                newName: "RefreshToken");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessions_RefreshTokenHash",
                table: "UserSessions",
                newName: "IX_UserSessions_RefreshToken");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEN9ATdcJNkWNAYA+pr58r13syNkh73T3qjBdMdAT7yTG0AYMxY2aNhXYlSEf0E1rHQ==");
        }
    }
}
