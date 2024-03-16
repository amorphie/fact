using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amorphie.fact.data.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalDeviceColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<DateTime>(
                name: "ActivationDate",
                table: "UserDevices",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "UserDevices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGoogleServiceAvailable",
                table: "UserDevices",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnApp",
                table: "UserDevices",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Manufacturer",
                table: "UserDevices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationDate",
                table: "UserDevices",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RemovalReason",
                table: "UserDevices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "UserDevices",
                type: "text",
                nullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "ActivationDate",
                table: "UserDevices");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "UserDevices");

            migrationBuilder.DropColumn(
                name: "IsGoogleServiceAvailable",
                table: "UserDevices");

            migrationBuilder.DropColumn(
                name: "IsOnApp",
                table: "UserDevices");

            migrationBuilder.DropColumn(
                name: "Manufacturer",
                table: "UserDevices");

            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                table: "UserDevices");

            migrationBuilder.DropColumn(
                name: "RemovalReason",
                table: "UserDevices");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "UserDevices");

        }
    }
}
