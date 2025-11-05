using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyDN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class claimSeedFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Claims",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Type", "Value" },
                values: new object[] { "Permissions", "PostAdd" });

            migrationBuilder.UpdateData(
                table: "Claims",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Type", "Value" },
                values: new object[] { "Permissions", "PostUpdate" });

            migrationBuilder.UpdateData(
                table: "Claims",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Type", "Value" },
                values: new object[] { "Permissions", "PostDelete" });

            migrationBuilder.UpdateData(
                table: "Claims",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Type", "Value" },
                values: new object[] { "Permissions", "PostGet" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Claims",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Type", "Value" },
                values: new object[] { "Post", "Add" });

            migrationBuilder.UpdateData(
                table: "Claims",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Type", "Value" },
                values: new object[] { "Post", "Update" });

            migrationBuilder.UpdateData(
                table: "Claims",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Type", "Value" },
                values: new object[] { "Post", "Delete" });

            migrationBuilder.UpdateData(
                table: "Claims",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Type", "Value" },
                values: new object[] { "Post", "Get" });
        }
    }
}
