using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyDN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class userRoleSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "RoleId", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[] { 1, 2, null, null, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
