using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dr.meow.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedDataAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "RoleName",
                value: "工程師");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "RoleName",
                value: "Team1主管");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "RoleName",
                value: "Team2主管");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[] { 5, "一般組員" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "Account", "CreatedAt" },
                values: new object[] { "enginee", new DateTime(2026, 3, 9, 16, 54, 38, 827, DateTimeKind.Local).AddTicks(2395) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 9, 16, 54, 38, 827, DateTimeKind.Local).AddTicks(2415));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 9, 16, 54, 38, 827, DateTimeKind.Local).AddTicks(2417));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 9, 16, 54, 38, 827, DateTimeKind.Local).AddTicks(2419));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Account", "AccountType", "CreatedAt", "Department", "Email", "GoogleId", "IsActive", "PasswordHash", "UserName" },
                values: new object[] { 5, "gmember", "User", new DateTime(2026, 3, 9, 16, 54, 38, 827, DateTimeKind.Local).AddTicks(2422), null, "super@drmeow.com", null, true, "123456", null });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 5, 5 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 5, 5 });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "RoleName",
                value: "單位主管");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "RoleName",
                value: "team1");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "RoleName",
                value: "team2");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "Account", "CreatedAt" },
                values: new object[] { "deptboss", new DateTime(2026, 3, 7, 14, 54, 3, 896, DateTimeKind.Local).AddTicks(325) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 7, 14, 54, 3, 896, DateTimeKind.Local).AddTicks(342));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 7, 14, 54, 3, 896, DateTimeKind.Local).AddTicks(343));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 7, 14, 54, 3, 896, DateTimeKind.Local).AddTicks(345));
        }
    }
}
