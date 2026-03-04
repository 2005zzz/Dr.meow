using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dr.meow.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestReviewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReviewAcceptanceContent",
                table: "RequestForms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewAcceptanceDate",
                table: "RequestForms",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReviewBenefitManDays",
                table: "RequestForms",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReviewBenefitRevenue",
                table: "RequestForms",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewRejectReason",
                table: "RequestForms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewSatisfactionNeed",
                table: "RequestForms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewSatisfactionOverall",
                table: "RequestForms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewSatisfactionStability",
                table: "RequestForms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewSecurityAssessment",
                table: "RequestForms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "RequestForms",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedBy",
                table: "RequestForms",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewAcceptanceContent",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "ReviewAcceptanceDate",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "ReviewBenefitManDays",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "ReviewBenefitRevenue",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "ReviewRejectReason",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "ReviewSatisfactionNeed",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "ReviewSatisfactionOverall",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "ReviewSatisfactionStability",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "ReviewSecurityAssessment",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "ReviewedBy",
                table: "RequestForms");
        }
    }
}
