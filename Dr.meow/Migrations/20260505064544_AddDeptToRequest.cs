using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dr.meow.Migrations
{
    /// <inheritdoc />
    public partial class AddDeptToRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "GoogleId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "RequestTickets",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "RequestTickets",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AiConsultLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AiResponse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiConsultLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    LinkUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
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
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    AiReviewComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AiRequirementScore = table.Column<int>(type: "int", nullable: true),
                    AiStabilityScore = table.Column<int>(type: "int", nullable: true),
                    AiOverallScore = table.Column<int>(type: "int", nullable: true),
                    AiSavedManDays = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AiRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
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
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Vulnerability",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ScheduledTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SystemCategory = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TicketCategory = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ChangeType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ImpactLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Dependency = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TestPlan = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RecoveryPlan = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequesterId = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastReviewerId = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vulnerability", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vulnerability_Users_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VulnerabilityLogs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VulnerabilityId = table.Column<int>(type: "int", nullable: false),
                    ReviewerId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StepName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VulnerabilityLogs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_VulnerabilityLogs_Vulnerability_VulnerabilityId",
                        column: x => x.VulnerabilityId,
                        principalTable: "Vulnerability",
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

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { 1, "工程師" },
                    { 2, "Team1主管" },
                    { 3, "Team2主管" },
                    { 4, "最高階主管" },
                    { 5, "Team1組員" },
                    { 6, "Team2組員" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Account", "AccountType", "CreatedAt", "Department", "Email", "GoogleId", "IsActive", "PasswordHash", "UserName" },
                values: new object[,]
                {
                    { 1, "enginee", "Admin", new DateTime(2026, 5, 5, 14, 45, 43, 68, DateTimeKind.Local).AddTicks(9490), null, "dept@drmeow.com", null, true, "123456", null },
                    { 2, "team1boss", "Admin", new DateTime(2026, 5, 5, 14, 45, 43, 68, DateTimeKind.Local).AddTicks(9494), "Team1", "team1@drmeow.com", null, true, "123456", null },
                    { 3, "team2boss", "Admin", new DateTime(2026, 5, 5, 14, 45, 43, 68, DateTimeKind.Local).AddTicks(9497), "Team2", "team2@drmeow.com", null, true, "123456", null },
                    { 4, "superboss", "Admin", new DateTime(2026, 5, 5, 14, 45, 43, 68, DateTimeKind.Local).AddTicks(9499), null, "super@drmeow.com", null, true, "123456", null },
                    { 5, "gmember1", "User", new DateTime(2026, 5, 5, 14, 45, 43, 68, DateTimeKind.Local).AddTicks(9501), "Team1", "user1@drmeow.com", null, true, "123456", null },
                    { 6, "gmember2", "User", new DateTime(2026, 5, 5, 14, 45, 43, 68, DateTimeKind.Local).AddTicks(9503), "Team2", "user2@drmeow.com", null, true, "123456", null }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 },
                    { 4, 4 },
                    { 5, 5 },
                    { 6, 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestTickets_RequesterId",
                table: "RequestTickets",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAuditLogs_ActorId",
                table: "RequestAuditLogs",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAuditLogs_RequestId",
                table: "RequestAuditLogs",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Vulnerability_RequesterId",
                table: "Vulnerability",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_VulnerabilityLogs_VulnerabilityId",
                table: "VulnerabilityLogs",
                column: "VulnerabilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestTickets_Users_RequesterId",
                table: "RequestTickets",
                column: "RequesterId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestUserInputs_RequestTickets_RequestId",
                table: "RequestUserInputs",
                column: "RequestId",
                principalTable: "RequestTickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestTickets_Users_RequesterId",
                table: "RequestTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestUserInputs_RequestTickets_RequestId",
                table: "RequestUserInputs");

            migrationBuilder.DropTable(
                name: "AiConsultLogs");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "RequestAiDetails");

            migrationBuilder.DropTable(
                name: "RequestAuditLogs");

            migrationBuilder.DropTable(
                name: "RequestStatuses");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "VulnerabilityLogs");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Vulnerability");

            migrationBuilder.DropIndex(
                name: "IX_RequestTickets_RequesterId",
                table: "RequestTickets");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6);

            migrationBuilder.DropColumn(
                name: "GoogleId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "RequestTickets");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "RequestTickets",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }
    }
}
