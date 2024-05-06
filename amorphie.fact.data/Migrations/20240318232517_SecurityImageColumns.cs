using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amorphie.fact.data.Migrations
{
    /// <inheritdoc />
    public partial class SecurityImageColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.DropColumn(
                name: "SecurityImage",
                table: "UserSecurityImages");

            migrationBuilder.AddColumn<bool>(
                name: "RequireChange",
                table: "UserSecurityImages",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SecurityImageId",
                table: "UserSecurityImages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "EnTitle",
                table: "SecurityImages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TrTitle",
                table: "SecurityImages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserSecurityImages_SecurityImageId",
                table: "UserSecurityImages",
                column: "SecurityImageId");

            // migrationBuilder.AddForeignKey(
            //     name: "FK_UserSecurityImages_SecurityImages_SecurityImageId",
            //     table: "UserSecurityImages",
            //     column: "SecurityImageId",
            //     principalTable: "SecurityImages",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSecurityImages_SecurityImages_SecurityImageId",
                table: "UserSecurityImages");

            migrationBuilder.DropIndex(
                name: "IX_UserSecurityImages_SecurityImageId",
                table: "UserSecurityImages");

            migrationBuilder.DropColumn(
                name: "RequireChange",
                table: "UserSecurityImages");

            migrationBuilder.DropColumn(
                name: "SecurityImageId",
                table: "UserSecurityImages");

            migrationBuilder.DropColumn(
                name: "EnTitle",
                table: "SecurityImages");

            migrationBuilder.DropColumn(
                name: "TrTitle",
                table: "SecurityImages");

            migrationBuilder.AddColumn<string>(
                name: "SecurityImage",
                table: "UserSecurityImages",
                type: "text",
                nullable: false,
                defaultValue: "");

        }
    }
}
