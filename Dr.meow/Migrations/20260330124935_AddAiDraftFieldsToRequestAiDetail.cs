using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dr.meow.Migrations
{
    /// <inheritdoc />
    public partial class AddAiDraftFieldsToRequestAiDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AiOverallScore",
                table: "RequestAiDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AiRequirementScore",
                table: "RequestAiDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AiRevenue",
                table: "RequestAiDetails",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AiReviewComment",
                table: "RequestAiDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AiSavedManDays",
                table: "RequestAiDetails",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AiStabilityScore",
                table: "RequestAiDetails",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 30, 20, 49, 34, 611, DateTimeKind.Local).AddTicks(9442));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 30, 20, 49, 34, 611, DateTimeKind.Local).AddTicks(9446));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 30, 20, 49, 34, 611, DateTimeKind.Local).AddTicks(9448));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 30, 20, 49, 34, 611, DateTimeKind.Local).AddTicks(9451));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 30, 20, 49, 34, 611, DateTimeKind.Local).AddTicks(9453));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 30, 20, 49, 34, 611, DateTimeKind.Local).AddTicks(9456));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiOverallScore",
                table: "RequestAiDetails");

            migrationBuilder.DropColumn(
                name: "AiRequirementScore",
                table: "RequestAiDetails");

            migrationBuilder.DropColumn(
                name: "AiRevenue",
                table: "RequestAiDetails");

            migrationBuilder.DropColumn(
                name: "AiReviewComment",
                table: "RequestAiDetails");

            migrationBuilder.DropColumn(
                name: "AiSavedManDays",
                table: "RequestAiDetails");

            migrationBuilder.DropColumn(
                name: "AiStabilityScore",
                table: "RequestAiDetails");

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
    }
}
