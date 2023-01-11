using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amorphie.user.data.Migrations
{
    /// <inheritdoc />
    public partial class UserTableEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserSecurityQuestions",
                keyColumn: "Id",
                keyValue: new Guid("633d9b29-81b8-4c11-a611-ef61d862ed51"));

            migrationBuilder.DeleteData(
                table: "UserTags",
                keyColumn: "Id",
                keyValue: new Guid("03832c57-53c1-4b72-b21f-bc2122b5b46c"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("750b7a20-839c-4d9b-bc7d-247c09d4d784"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "LastLoginDate", "ModifiedDate", "Name", "Password", "State", "Surname", "TcNo" },
                values: new object[] { new Guid("2a30108b-88b5-4903-ab7a-90b691cdc5a5"), null, null, "Damla", "123", null, "Erhan", "12345678912" });

            migrationBuilder.InsertData(
                table: "UserSecurityQuestions",
                columns: new[] { "Id", "SecurityQuestion", "UserId" },
                values: new object[] { new Guid("76f62669-5301-4998-bae3-89f85aa0a07c"), "en sevdiğiniz araba modeli", new Guid("2a30108b-88b5-4903-ab7a-90b691cdc5a5") });

            migrationBuilder.InsertData(
                table: "UserTags",
                columns: new[] { "Id", "Name", "UserId" },
                values: new object[] { new Guid("f773ec73-cf51-4800-94ac-a950301cd7b2"), "user-list-get", new Guid("2a30108b-88b5-4903-ab7a-90b691cdc5a5") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserSecurityQuestions",
                keyColumn: "Id",
                keyValue: new Guid("76f62669-5301-4998-bae3-89f85aa0a07c"));

            migrationBuilder.DeleteData(
                table: "UserTags",
                keyColumn: "Id",
                keyValue: new Guid("f773ec73-cf51-4800-94ac-a950301cd7b2"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2a30108b-88b5-4903-ab7a-90b691cdc5a5"));

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
        }
    }
}
