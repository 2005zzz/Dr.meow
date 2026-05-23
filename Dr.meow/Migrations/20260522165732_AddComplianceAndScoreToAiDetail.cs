using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dr.meow.Migrations
{
    /// <inheritdoc />
    public partial class AddComplianceAndScoreToAiDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ComplianceStatus",
                table: "VulnerabilityAiDetail",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PriorityScore",
                table: "VulnerabilityAiDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 23, 0, 57, 32, 227, DateTimeKind.Local).AddTicks(6515));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 23, 0, 57, 32, 227, DateTimeKind.Local).AddTicks(6518));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 23, 0, 57, 32, 227, DateTimeKind.Local).AddTicks(6520));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 23, 0, 57, 32, 227, DateTimeKind.Local).AddTicks(6522));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 23, 0, 57, 32, 227, DateTimeKind.Local).AddTicks(6524));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 23, 0, 57, 32, 227, DateTimeKind.Local).AddTicks(6526));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComplianceStatus",
                table: "VulnerabilityAiDetail");

            migrationBuilder.DropColumn(
                name: "PriorityScore",
                table: "VulnerabilityAiDetail");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 23, 0, 4, 45, 214, DateTimeKind.Local).AddTicks(6765));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 23, 0, 4, 45, 214, DateTimeKind.Local).AddTicks(6767));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 23, 0, 4, 45, 214, DateTimeKind.Local).AddTicks(6769));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 23, 0, 4, 45, 214, DateTimeKind.Local).AddTicks(6771));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 23, 0, 4, 45, 214, DateTimeKind.Local).AddTicks(6773));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 23, 0, 4, 45, 214, DateTimeKind.Local).AddTicks(6775));
        }
    }
}
