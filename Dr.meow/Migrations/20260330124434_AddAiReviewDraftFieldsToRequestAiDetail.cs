using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dr.meow.Migrations
{
    /// <inheritdoc />
    public partial class AddAiReviewDraftFieldsToRequestAiDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 30, 20, 44, 31, 464, DateTimeKind.Local).AddTicks(9069));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 30, 20, 44, 31, 464, DateTimeKind.Local).AddTicks(9076));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 30, 20, 44, 31, 464, DateTimeKind.Local).AddTicks(9081));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 30, 20, 44, 31, 464, DateTimeKind.Local).AddTicks(9085));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 30, 20, 44, 31, 464, DateTimeKind.Local).AddTicks(9089));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 30, 20, 44, 31, 464, DateTimeKind.Local).AddTicks(9094));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 29, 16, 37, 7, 583, DateTimeKind.Local).AddTicks(4462));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 29, 16, 37, 7, 583, DateTimeKind.Local).AddTicks(4465));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 29, 16, 37, 7, 583, DateTimeKind.Local).AddTicks(4467));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 29, 16, 37, 7, 583, DateTimeKind.Local).AddTicks(4468));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 29, 16, 37, 7, 583, DateTimeKind.Local).AddTicks(4470));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 29, 16, 37, 7, 583, DateTimeKind.Local).AddTicks(4472));
        }
    }
}
