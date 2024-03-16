using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amorphie.fact.data.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "UserSecurityQuestions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessDate",
                table: "UserSecurityQuestions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastVerificationDate",
                table: "UserSecurityQuestions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "UserSecurityQuestions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SecurityQuestions",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "SecurityQuestions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "SecurityQuestions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValueTypeClr",
                table: "SecurityQuestions",
                type: "text",
                nullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "UserSecurityQuestions");

            migrationBuilder.DropColumn(
                name: "LastAccessDate",
                table: "UserSecurityQuestions");

            migrationBuilder.DropColumn(
                name: "LastVerificationDate",
                table: "UserSecurityQuestions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserSecurityQuestions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SecurityQuestions");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "SecurityQuestions");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "SecurityQuestions");

            migrationBuilder.DropColumn(
                name: "ValueTypeClr",
                table: "SecurityQuestions");

        }
    }
}
