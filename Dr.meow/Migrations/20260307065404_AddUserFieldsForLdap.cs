using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dr.meow.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFieldsForLdap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssignedEngineerId",
                table: "RequestForms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CrossReviewerComment",
                table: "RequestForms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CrossReviewerId",
                table: "RequestForms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EngineerTestResult",
                table: "RequestForms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormType",
                table: "RequestForms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Department", "UserName" },
                values: new object[] { new DateTime(2026, 3, 7, 14, 54, 3, 896, DateTimeKind.Local).AddTicks(325), null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Department", "UserName" },
                values: new object[] { new DateTime(2026, 3, 7, 14, 54, 3, 896, DateTimeKind.Local).AddTicks(342), null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Department", "UserName" },
                values: new object[] { new DateTime(2026, 3, 7, 14, 54, 3, 896, DateTimeKind.Local).AddTicks(343), null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Department", "UserName" },
                values: new object[] { new DateTime(2026, 3, 7, 14, 54, 3, 896, DateTimeKind.Local).AddTicks(345), null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AssignedEngineerId",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "CrossReviewerComment",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "CrossReviewerId",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "EngineerTestResult",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "FormType",
                table: "RequestForms");
        }
    }
}
