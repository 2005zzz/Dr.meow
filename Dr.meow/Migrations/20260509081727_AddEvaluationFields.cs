using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dr.meow.Migrations
{
    /// <inheritdoc />
    public partial class AddEvaluationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "RequestTickets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Department",
                table: "RequestTickets",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "ExpectedBenefits",
                table: "RequestTickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedCompletionDate",
                table: "RequestTickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "RequestTickets",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestType",
                table: "RequestTickets",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SystemCategory",
                table: "RequestTickets",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 9, 16, 17, 26, 671, DateTimeKind.Local).AddTicks(1248));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 9, 16, 17, 26, 671, DateTimeKind.Local).AddTicks(1250));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 9, 16, 17, 26, 671, DateTimeKind.Local).AddTicks(1252));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 9, 16, 17, 26, 671, DateTimeKind.Local).AddTicks(1254));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 9, 16, 17, 26, 671, DateTimeKind.Local).AddTicks(1256));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 9, 16, 17, 26, 671, DateTimeKind.Local).AddTicks(1257));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpectedBenefits",
                table: "RequestTickets");

            migrationBuilder.DropColumn(
                name: "ExpectedCompletionDate",
                table: "RequestTickets");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "RequestTickets");

            migrationBuilder.DropColumn(
                name: "RequestType",
                table: "RequestTickets");

            migrationBuilder.DropColumn(
                name: "SystemCategory",
                table: "RequestTickets");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "RequestTickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Department",
                table: "RequestTickets",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 5, 14, 45, 43, 68, DateTimeKind.Local).AddTicks(9490));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 5, 14, 45, 43, 68, DateTimeKind.Local).AddTicks(9494));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 5, 14, 45, 43, 68, DateTimeKind.Local).AddTicks(9497));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 5, 14, 45, 43, 68, DateTimeKind.Local).AddTicks(9499));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 5, 14, 45, 43, 68, DateTimeKind.Local).AddTicks(9501));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 5, 14, 45, 43, 68, DateTimeKind.Local).AddTicks(9503));
        }
    }
}
