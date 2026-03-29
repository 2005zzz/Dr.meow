using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dr.meow.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToUserInput : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestForms");

            migrationBuilder.CreateTable(
                name: "RequestStatuses",
                columns: table => new
                {
                    StatusId = table.Column<byte>(type: "tinyint", nullable: false),
                    StatusName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestStatuses", x => x.StatusId);
                });

            migrationBuilder.CreateTable(
                name: "RequestTickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequesterId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestTickets_Users_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestAiDetails",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    IsITRelated = table.Column<bool>(type: "bit", nullable: false),
                    RefinedTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefinedDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityAssessment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AiReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestAiDetails", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_RequestAiDetails_RequestTickets_RequestId",
                        column: x => x.RequestId,
                        principalTable: "RequestTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    ActorId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetUserId = table.Column<int>(type: "int", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestAuditLogs_RequestTickets_RequestId",
                        column: x => x.RequestId,
                        principalTable: "RequestTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestAuditLogs_Users_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestUserInputs",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contact = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SystemCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Benefit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpectedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestUserInputs", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_RequestUserInputs_RequestTickets_RequestId",
                        column: x => x.RequestId,
                        principalTable: "RequestTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "RequestStatuses",
                columns: new[] { "StatusId", "Description", "StatusName" },
                values: new object[,]
                {
                    { (byte)0, "等待 AI 分析", "PendingAI" },
                    { (byte)1, "等待主管審核", "PendingReview" },
                    { (byte)2, "工程師開發中", "InDevelopment" },
                    { (byte)3, "需求已結案", "Completed" },
                    { (byte)4, "需求已被拒絕", "Rejected" }
                });

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
                columns: new[] { "CreatedAt", "Email" },
                values: new object[] { new DateTime(2026, 3, 29, 16, 37, 7, 583, DateTimeKind.Local).AddTicks(4470), "user1@drmeow.com" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "Email" },
                values: new object[] { new DateTime(2026, 3, 29, 16, 37, 7, 583, DateTimeKind.Local).AddTicks(4472), "user2@drmeow.com" });

            migrationBuilder.CreateIndex(
                name: "IX_RequestAuditLogs_ActorId",
                table: "RequestAuditLogs",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAuditLogs_RequestId",
                table: "RequestAuditLogs",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestTickets_RequesterId",
                table: "RequestTickets",
                column: "RequesterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestAiDetails");

            migrationBuilder.DropTable(
                name: "RequestAuditLogs");

            migrationBuilder.DropTable(
                name: "RequestStatuses");

            migrationBuilder.DropTable(
                name: "RequestUserInputs");

            migrationBuilder.DropTable(
                name: "RequestTickets");

            migrationBuilder.CreateTable(
                name: "RequestForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcceptanceContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AiPass = table.Column<bool>(type: "bit", nullable: true),
                    AiReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AiReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedEngineerId = table.Column<int>(type: "int", nullable: true),
                    Contact = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CrossReviewerComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CrossReviewerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeveloperId = table.Column<int>(type: "int", nullable: true),
                    DeveloperName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EngineerTestResult = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FormType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProgressRate = table.Column<int>(type: "int", nullable: false),
                    RequesterId = table.Column<int>(type: "int", nullable: false),
                    ReviewAcceptanceContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewAcceptanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewBenefitManDays = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ReviewBenefitRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ReviewRejectReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewSatisfactionNeed = table.Column<int>(type: "int", nullable: true),
                    ReviewSatisfactionOverall = table.Column<int>(type: "int", nullable: true),
                    ReviewSatisfactionStability = table.Column<int>(type: "int", nullable: true),
                    ReviewSecurityAssessment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityAssessment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SystemCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TicketNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestForms", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 17, 11, 54, 19, 641, DateTimeKind.Local).AddTicks(7350));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 17, 11, 54, 19, 641, DateTimeKind.Local).AddTicks(7354));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 17, 11, 54, 19, 641, DateTimeKind.Local).AddTicks(7357));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 17, 11, 54, 19, 641, DateTimeKind.Local).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "Email" },
                values: new object[] { new DateTime(2026, 3, 17, 11, 54, 19, 641, DateTimeKind.Local).AddTicks(7363), "super@drmeow.com" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "Email" },
                values: new object[] { new DateTime(2026, 3, 17, 11, 54, 19, 641, DateTimeKind.Local).AddTicks(7366), "super@drmeow.com" });
        }
    }
}
