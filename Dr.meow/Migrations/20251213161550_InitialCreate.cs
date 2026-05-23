using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dr.meow.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        // <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        //    migrationBuilder.CreateTable(
        //        name: "Vulnerability",
        //        columns: table => new
        //        {
        //            Id = table.Column<int>(type: "int", nullable: false)
        //                .Annotation("SqlServer:Identity", "1, 1"),
        //            TicketNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
        //            FoundDate = table.Column<DateTime>(type: "datetime2", nullable: false),
        //            ScheduledTime = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
        //            Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
        //            SystemCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
        //            TicketCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
        //            ChangeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
        //            Dependency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
        //            ImpactLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
        //            Severity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
        //            TestPlan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
        //            RecoveryPlan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
        //            Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
        //            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
        //            AssignedTo = table.Column<string>(type: "nvarchar(max)", nullable: true)
        //        },
        //        constraints: table =>
        //        {
        //            table.PrimaryKey("PK_Vulnerability", x => x.Id);
        //        });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vulnerability");
        }
    }
}
