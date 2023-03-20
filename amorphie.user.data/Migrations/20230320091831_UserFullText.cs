using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace amorphie.user.data.Migrations
{
    /// <inheritdoc />
    public partial class UserFullText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SecurityImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityImages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SecurityQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Question = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    EMail = table.Column<string>(type: "text", nullable: false),
                    CountryCode = table.Column<int>(type: "integer", nullable: true),
                    Prefix = table.Column<int>(type: "integer", nullable: true),
                    Number = table.Column<string>(type: "text", nullable: true),
                    Reference = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<string>(type: "text", nullable: false),
                    Salt = table.Column<string>(type: "text", nullable: false),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false, computedColumnSql: "to_tsvector('english', coalesce(\"Reference\", '') || ' ' || coalesce(\"EMail\", '') || ' ' || coalesce(\"FirstName\", '') || ' ' || coalesce(\"LastName\", '') || ' ' || coalesce(\"State\", '') || ' ' || coalesce(\"Number\", ''))", stored: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDevices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    TokenId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDevices_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPasswords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HashedPassword = table.Column<string>(type: "text", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: true),
                    MustResetPassword = table.Column<bool>(type: "boolean", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPasswords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPasswords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSecurityImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SecurityImage = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSecurityImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSecurityImages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSecurityQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SecurityAnswer = table.Column<string>(type: "text", nullable: false),
                    SecurityQuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSecurityQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSecurityQuestions_SecurityQuestions_SecurityQuestionId",
                        column: x => x.SecurityQuestionId,
                        principalTable: "SecurityQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    Tag = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true)
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
                table: "SecurityImages",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "Image", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf" },
                values: new object[] { new Guid("a9ef2ccc-a7ec-472d-b540-6e2e54525943"), new DateTime(2023, 3, 20, 9, 18, 31, 297, DateTimeKind.Utc).AddTicks(1194), new Guid("17e03ac0-14a8-48ba-b919-558611410f1a"), new Guid("d910c412-daa7-4869-9658-adc2ef61deac"), "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", new DateTime(2023, 3, 20, 9, 18, 31, 297, DateTimeKind.Utc).AddTicks(1198), new Guid("e0ca5481-377a-4f12-a2e6-e5b789709969"), new Guid("11765259-f307-4416-b3e5-cc87e86ad9d5") });

            migrationBuilder.InsertData(
                table: "SecurityQuestions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "Question" },
                values: new object[] { new Guid("705068e1-802b-4a78-8589-c56ce22bd97d"), new DateTime(2023, 3, 20, 9, 18, 31, 297, DateTimeKind.Utc).AddTicks(1227), new Guid("791135b4-b168-4590-8f41-25142c73bbc0"), new Guid("38a38cc1-a5d9-4f7b-90e1-d31d6382b959"), new DateTime(2023, 3, 20, 9, 18, 31, 297, DateTimeKind.Utc).AddTicks(1228), new Guid("6000d5a7-a4e7-4bea-b280-4a7176cdcac6"), new Guid("337f760d-fd66-4a06-8a9f-9fb3b5cc3478"), "ilk öğretmenin adı" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CountryCode", "Number", "Prefix", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "EMail", "FirstName", "LastName", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "Reference", "Salt", "State" },
                values: new object[] { new Guid("9b7ed9fb-d2dc-4785-b365-fd5062b7e846"), 90, "1234564", 530, new DateTime(2023, 3, 20, 9, 18, 31, 297, DateTimeKind.Utc).AddTicks(946), new Guid("ef69bd8b-5702-4cce-a776-668cd924aa3e"), new Guid("c036a97f-db83-4c0e-9775-f8a97d06b539"), "test@gmail.com", "Damla", "Erhan", new DateTime(2023, 3, 20, 9, 18, 31, 297, DateTimeKind.Utc).AddTicks(949), new Guid("6f93a688-c62e-491b-8ff0-21609a8fd9de"), new Guid("37499c0e-6d6e-4f1d-9375-642a21416c51"), "12345678912", "fertrtretregfdgffd", "New" });

            migrationBuilder.InsertData(
                table: "UserDevices",
                columns: new[] { "Id", "ClientId", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "DeviceId", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "TokenId", "UserId" },
                values: new object[] { new Guid("b3545832-60c6-4ba1-aa25-400c0c3bf517"), new Guid("8c9354e8-ee7e-4bc5-9018-667af65633a6"), new DateTime(2023, 3, 20, 9, 18, 31, 297, DateTimeKind.Utc).AddTicks(1176), new Guid("f4733a2b-171c-466f-b133-a6a1aa9af2d5"), new Guid("9139fb75-1c07-46cb-b12c-ec00ce0e0790"), 123, new DateTime(2023, 3, 20, 9, 18, 31, 297, DateTimeKind.Utc).AddTicks(1177), new Guid("7a5e20a8-eddb-455b-b391-05fee0b4ad60"), new Guid("2ac8aa14-aa1b-49f7-86ea-093094ce5abc"), new Guid("5db260bc-f2b6-479f-90ad-d46245a62360"), new Guid("9b7ed9fb-d2dc-4785-b365-fd5062b7e846") });

            migrationBuilder.InsertData(
                table: "UserPasswords",
                columns: new[] { "Id", "AccessFailedCount", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "HashedPassword", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "MustResetPassword", "UserId" },
                values: new object[] { new Guid("835aa957-e4c2-4300-b3ae-707f95219b38"), null, new DateTime(2023, 3, 20, 9, 18, 31, 297, DateTimeKind.Utc).AddTicks(1261), new Guid("364a8590-2e53-42a3-9abc-7d35dee3a3da"), new Guid("0d1dad81-e02f-4c3c-81c6-58a1df40508d"), "", new DateTime(2023, 3, 20, 9, 18, 31, 297, DateTimeKind.Utc).AddTicks(1262), new Guid("9742c1df-f5ba-48c9-8bd2-edabd45e44da"), new Guid("918b42ed-c8e4-4cb3-af25-e3a58061d235"), null, new Guid("9b7ed9fb-d2dc-4785-b365-fd5062b7e846") });

            migrationBuilder.InsertData(
                table: "UserSecurityImages",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "SecurityImage", "UserId" },
                values: new object[] { new Guid("f6d09b50-b9d6-4c69-8465-bd236d9aa28c"), new DateTime(2023, 3, 20, 9, 18, 31, 297, DateTimeKind.Utc).AddTicks(1210), new Guid("67546474-4aed-477f-8e9b-9c7e5fe555fb"), new Guid("3b727ae1-7af3-4a98-83ed-e49a53e4d067"), new DateTime(2023, 3, 20, 9, 18, 31, 297, DateTimeKind.Utc).AddTicks(1212), new Guid("841bcc1d-f55e-492c-a9f3-a6b1aecf743e"), new Guid("a1bf3f85-feb0-492c-bbfd-c77e438c688d"), "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", new Guid("9b7ed9fb-d2dc-4785-b365-fd5062b7e846") });

            migrationBuilder.InsertData(
                table: "UserSecurityQuestions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "SecurityAnswer", "SecurityQuestionId", "UserId" },
                values: new object[] { new Guid("0446570d-dcdd-4307-b47f-95c9b34e64aa"), new DateTime(2023, 3, 20, 9, 18, 31, 297, DateTimeKind.Utc).AddTicks(1244), new Guid("1855cd09-c14a-45d8-b71c-db97d796814a"), new Guid("f9cc88ce-ca10-424d-b050-557703096eb0"), new DateTime(2023, 3, 20, 9, 18, 31, 297, DateTimeKind.Utc).AddTicks(1245), new Guid("937fc8ed-b13c-44fe-9b03-c0b7d2fa55c1"), new Guid("36a55df6-5011-40a3-96f3-f87e957909bb"), "test", new Guid("705068e1-802b-4a78-8589-c56ce22bd97d"), new Guid("9b7ed9fb-d2dc-4785-b365-fd5062b7e846") });

            migrationBuilder.InsertData(
                table: "UserTags",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "Tag", "UserId" },
                values: new object[] { new Guid("3020685c-2bed-4f24-a955-7e2e6a882a07"), new DateTime(2023, 3, 20, 9, 18, 31, 297, DateTimeKind.Utc).AddTicks(1156), new Guid("30380740-176f-4e2d-81ad-3e2e3ab709de"), new Guid("bfc430a0-ea4e-489e-b889-18b81e22a97f"), new DateTime(2023, 3, 20, 9, 18, 31, 297, DateTimeKind.Utc).AddTicks(1157), new Guid("8362fbf5-425a-4e4f-8485-aaf5999bf004"), new Guid("c1134dd5-edeb-46e6-9569-359a44230a4c"), "user-list-get", new Guid("9b7ed9fb-d2dc-4785-b365-fd5062b7e846") });

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_UserId",
                table: "UserDevices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPasswords_UserId",
                table: "UserPasswords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Reference",
                table: "Users",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_SearchVector",
                table: "Users",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_UserSecurityImages_UserId",
                table: "UserSecurityImages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSecurityQuestions_SecurityQuestionId",
                table: "UserSecurityQuestions",
                column: "SecurityQuestionId");

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
                name: "SecurityImages");

            migrationBuilder.DropTable(
                name: "UserDevices");

            migrationBuilder.DropTable(
                name: "UserPasswords");

            migrationBuilder.DropTable(
                name: "UserSecurityImages");

            migrationBuilder.DropTable(
                name: "UserSecurityQuestions");

            migrationBuilder.DropTable(
                name: "UserTags");

            migrationBuilder.DropTable(
                name: "SecurityQuestions");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
