using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amorphie.user.data.Migrations
{
    /// <inheritdoc />
    public partial class TableCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Surname = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    TcNo = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    State = table.Column<int>(type: "integer", nullable: true),
                    LastLoginDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSecurityQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SecurityQuestion = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSecurityQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSecurityQuestions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTags_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "LastLoginDate", "ModifiedDate", "Name", "Password", "State", "Surname", "TcNo" },
                values: new object[] { new Guid("750b7a20-839c-4d9b-bc7d-247c09d4d784"), null, null, "Damla", "12345", null, "Erhan", "12345" });

            migrationBuilder.InsertData(
                table: "UserSecurityQuestions",
                columns: new[] { "Id", "SecurityQuestion", "UserId" },
                values: new object[] { new Guid("633d9b29-81b8-4c11-a611-ef61d862ed51"), "en sevdiğiniz araba", new Guid("750b7a20-839c-4d9b-bc7d-247c09d4d784") });

            migrationBuilder.InsertData(
                table: "UserTags",
                columns: new[] { "Id", "Name", "UserId" },
                values: new object[] { new Guid("03832c57-53c1-4b72-b21f-bc2122b5b46c"), "user-list-get", new Guid("750b7a20-839c-4d9b-bc7d-247c09d4d784") });

            migrationBuilder.CreateIndex(
                name: "IX_UserSecurityQuestions_UserId",
                table: "UserSecurityQuestions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTags_UserId",
                table: "UserTags",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSecurityQuestions");

            migrationBuilder.DropTable(
                name: "UserTags");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
