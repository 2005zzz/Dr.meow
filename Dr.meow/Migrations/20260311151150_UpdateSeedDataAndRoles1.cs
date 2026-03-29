using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dr.meow.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedDataAndRoles1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "RoleName",
                value: "Team1組員");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[] { 6, "Team2組員" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 11, 23, 11, 50, 189, DateTimeKind.Local).AddTicks(5803));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Department" },
                values: new object[] { new DateTime(2026, 3, 11, 23, 11, 50, 189, DateTimeKind.Local).AddTicks(5822), "Team1" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Department" },
                values: new object[] { new DateTime(2026, 3, 11, 23, 11, 50, 189, DateTimeKind.Local).AddTicks(5825), "Team2" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 11, 23, 11, 50, 189, DateTimeKind.Local).AddTicks(5826));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                columns: new[] { "Account", "CreatedAt", "Department" },
                values: new object[] { "gmember1", new DateTime(2026, 3, 11, 23, 11, 50, 189, DateTimeKind.Local).AddTicks(5827), "Team1" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Account", "AccountType", "CreatedAt", "Department", "Email", "GoogleId", "IsActive", "PasswordHash", "UserName" },
                values: new object[] { 6, "gmember2", "User", new DateTime(2026, 3, 11, 23, 11, 50, 189, DateTimeKind.Local).AddTicks(5829), "Team2", "super@drmeow.com", null, true, "123456", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "RoleName",
                value: "一般組員");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 9, 16, 54, 38, 827, DateTimeKind.Local).AddTicks(2395));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Department" },
                values: new object[] { new DateTime(2026, 3, 9, 16, 54, 38, 827, DateTimeKind.Local).AddTicks(2415), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Department" },
                values: new object[] { new DateTime(2026, 3, 9, 16, 54, 38, 827, DateTimeKind.Local).AddTicks(2417), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 9, 16, 54, 38, 827, DateTimeKind.Local).AddTicks(2419));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                columns: new[] { "Account", "CreatedAt", "Department" },
                values: new object[] { "gmember", new DateTime(2026, 3, 9, 16, 54, 38, 827, DateTimeKind.Local).AddTicks(2422), null });
        }
    }
}
