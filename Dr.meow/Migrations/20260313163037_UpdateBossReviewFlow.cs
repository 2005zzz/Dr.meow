using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dr.meow.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBossReviewFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CrossBossComment",
                table: "Vulnerability",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CrossBossId",
                table: "Vulnerability",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentStep",
                table: "Vulnerability",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FormType",
                table: "Vulnerability",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TeamBossComment",
                table: "Vulnerability",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamBossId",
                table: "Vulnerability",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcceptanceContent",
                table: "RequestForms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeveloperId",
                table: "RequestForms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeveloperName",
                table: "RequestForms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EstStartDate",
                table: "RequestForms",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecurityAssessment",
                table: "RequestForms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 6, 6 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 0, 30, 36, 543, DateTimeKind.Local).AddTicks(363));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 0, 30, 36, 543, DateTimeKind.Local).AddTicks(384));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 0, 30, 36, 543, DateTimeKind.Local).AddTicks(386));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 0, 30, 36, 543, DateTimeKind.Local).AddTicks(387));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 0, 30, 36, 543, DateTimeKind.Local).AddTicks(389));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 0, 30, 36, 543, DateTimeKind.Local).AddTicks(390));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 6, 6 });

            migrationBuilder.DropColumn(
                name: "CrossBossComment",
                table: "Vulnerability");

            migrationBuilder.DropColumn(
                name: "CrossBossId",
                table: "Vulnerability");

            migrationBuilder.DropColumn(
                name: "CurrentStep",
                table: "Vulnerability");

            migrationBuilder.DropColumn(
                name: "FormType",
                table: "Vulnerability");

            migrationBuilder.DropColumn(
                name: "TeamBossComment",
                table: "Vulnerability");

            migrationBuilder.DropColumn(
                name: "TeamBossId",
                table: "Vulnerability");

            migrationBuilder.DropColumn(
                name: "AcceptanceContent",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "DeveloperId",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "DeveloperName",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "EstStartDate",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "SecurityAssessment",
                table: "RequestForms");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 12, 2, 13, 14, 849, DateTimeKind.Local).AddTicks(798));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 12, 2, 13, 14, 849, DateTimeKind.Local).AddTicks(818));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 12, 2, 13, 14, 849, DateTimeKind.Local).AddTicks(820));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 12, 2, 13, 14, 849, DateTimeKind.Local).AddTicks(821));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 12, 2, 13, 14, 849, DateTimeKind.Local).AddTicks(822));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 12, 2, 13, 14, 849, DateTimeKind.Local).AddTicks(824));
        }
    }
}
