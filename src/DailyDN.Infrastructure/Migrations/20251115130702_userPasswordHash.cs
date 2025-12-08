using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyDN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class userPasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEN9ATdcJNkWNAYA+pr58r13syNkh73T3qjBdMdAT7yTG0AYMxY2aNhXYlSEf0E1rHQ==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "hashpassword");
        }
    }
}
