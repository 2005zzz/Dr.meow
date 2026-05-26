using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dr.meow.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationScheduleSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationScheduleSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Hour = table.Column<int>(type: "int", nullable: false),
                    Minute = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationScheduleSettings", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 26, 6, 57, 34, 331, DateTimeKind.Local).AddTicks(882));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 26, 6, 57, 34, 331, DateTimeKind.Local).AddTicks(885));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 26, 6, 57, 34, 331, DateTimeKind.Local).AddTicks(888));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 26, 6, 57, 34, 331, DateTimeKind.Local).AddTicks(890));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 26, 6, 57, 34, 331, DateTimeKind.Local).AddTicks(893));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 26, 6, 57, 34, 331, DateTimeKind.Local).AddTicks(895));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationScheduleSettings");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 6, 5, 21, 836, DateTimeKind.Local).AddTicks(6890));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 6, 5, 21, 836, DateTimeKind.Local).AddTicks(6895));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 6, 5, 21, 836, DateTimeKind.Local).AddTicks(6899));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 6, 5, 21, 836, DateTimeKind.Local).AddTicks(6902));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 6, 5, 21, 836, DateTimeKind.Local).AddTicks(6906));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 6, 5, 21, 836, DateTimeKind.Local).AddTicks(6909));
        }
    }
}
