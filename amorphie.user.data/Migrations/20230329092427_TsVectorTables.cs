using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace amorphie.user.data.Migrations
{
    /// <inheritdoc />
    public partial class TsVectorTables : Migration
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
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false, computedColumnSql: "to_tsvector('english', coalesce(\"Reference\", '') || ' ' || coalesce(\"EMail\", '') || ' ' || coalesce(\"FirstName\", '') || ' ' || coalesce(\"LastName\", '') || ' ' || coalesce(\"Number\", ''))", stored: true),
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
                    Status = table.Column<int>(type: "integer", nullable: false),
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
                values: new object[] { new Guid("6b8e63e7-531c-4fa6-940f-3744545ba994"), new DateTime(2023, 3, 29, 9, 24, 26, 910, DateTimeKind.Utc).AddTicks(2245), new Guid("1a6ab381-200b-494f-ba21-aad861649f7a"), new Guid("9a11ff22-649c-47d4-a1f1-d1b689e50188"), "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", new DateTime(2023, 3, 29, 9, 24, 26, 910, DateTimeKind.Utc).AddTicks(2246), new Guid("f684d298-40a8-4e1a-8573-3fdd8c43fc09"), new Guid("59d7f9ef-c644-41b9-9fc7-e097bc6aa02b") });

            migrationBuilder.InsertData(
                table: "SecurityQuestions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "Question" },
                values: new object[] { new Guid("700c244d-ebc9-48d9-8073-46b00d69b7d3"), new DateTime(2023, 3, 29, 9, 24, 26, 910, DateTimeKind.Utc).AddTicks(2275), new Guid("74bb8c55-292a-4182-b8a5-1ce8c8113891"), new Guid("f501785f-f0cb-421e-9900-bace4368e314"), new DateTime(2023, 3, 29, 9, 24, 26, 910, DateTimeKind.Utc).AddTicks(2278), new Guid("1586ded0-28c4-4233-94ab-966beb3244a0"), new Guid("0c5291f0-b326-48d4-ad9c-cfc2d09034da"), "ilk öğretmenin adı" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CountryCode", "Number", "Prefix", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "EMail", "FirstName", "LastName", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "Reference", "Salt", "State" },
                values: new object[] { new Guid("8a51ec25-b63b-42bc-b913-fd3d7526e072"), 90, "1234564", 530, new DateTime(2023, 3, 29, 9, 24, 26, 910, DateTimeKind.Utc).AddTicks(1995), new Guid("44a19f1d-7446-4107-a2e1-febffb8cb8bb"), new Guid("e6bd35aa-73e2-440f-998a-13e08c1adea2"), "test@gmail.com", "Damla", "Erhan", new DateTime(2023, 3, 29, 9, 24, 26, 910, DateTimeKind.Utc).AddTicks(2002), new Guid("b436b274-b4aa-43fb-a80d-9cde141ce17a"), new Guid("2b65003d-10c7-4a92-a706-022828f05311"), "12345678912", "fertrtretregfdgffd", "New" });

            migrationBuilder.InsertData(
                table: "UserDevices",
                columns: new[] { "Id", "ClientId", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "DeviceId", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "Status", "TokenId", "UserId" },
                values: new object[] { new Guid("e8f36277-1cfe-4c63-b360-4555b77449ab"), new Guid("d69e5862-48ea-4e24-8675-b76b1c2e79dd"), new DateTime(2023, 3, 29, 9, 24, 26, 910, DateTimeKind.Utc).AddTicks(2223), new Guid("1827f97c-df5a-46b3-a356-dc25038a612a"), new Guid("57989328-8b86-4db8-965d-236ba08d7f2f"), 123, new DateTime(2023, 3, 29, 9, 24, 26, 910, DateTimeKind.Utc).AddTicks(2224), new Guid("87aa3435-31a7-4c4b-b302-6e4c1d014252"), new Guid("97e805cf-2874-495a-a036-bd529624a48d"), 1, new Guid("29c52707-da0e-4bcd-8c80-e678cea8b4e5"), new Guid("8a51ec25-b63b-42bc-b913-fd3d7526e072") });

            migrationBuilder.InsertData(
                table: "UserPasswords",
                columns: new[] { "Id", "AccessFailedCount", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "HashedPassword", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "MustResetPassword", "UserId" },
                values: new object[] { new Guid("3b673dd8-c463-4a4d-bfbe-39e7aa172d27"), null, new DateTime(2023, 3, 29, 9, 24, 26, 910, DateTimeKind.Utc).AddTicks(2312), new Guid("ebf9aeae-7c18-4509-8877-3f40393d3227"), new Guid("f74cf7cf-caa6-4051-82c5-b92c77995d7f"), "", new DateTime(2023, 3, 29, 9, 24, 26, 910, DateTimeKind.Utc).AddTicks(2314), new Guid("3c4c2958-3047-4e0c-be09-d6869732329a"), new Guid("1bcb7a74-f183-4c48-aa61-39dd8e7a0f8b"), null, new Guid("8a51ec25-b63b-42bc-b913-fd3d7526e072") });

            migrationBuilder.InsertData(
                table: "UserSecurityImages",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "SecurityImage", "UserId" },
                values: new object[] { new Guid("7f13a0ad-70ec-4fd8-a167-4fff21fcca19"), new DateTime(2023, 3, 29, 9, 24, 26, 910, DateTimeKind.Utc).AddTicks(2261), new Guid("8a4cb4f8-1bf5-4159-872e-3a4d29d5b931"), new Guid("05554731-67c0-4681-ac7a-e7ec72143389"), new DateTime(2023, 3, 29, 9, 24, 26, 910, DateTimeKind.Utc).AddTicks(2262), new Guid("2c5ad9e6-9b79-4337-9b1d-3abe6e6ce852"), new Guid("5f603d3b-5202-44d3-bc42-d509a883e195"), "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", new Guid("8a51ec25-b63b-42bc-b913-fd3d7526e072") });

            migrationBuilder.InsertData(
                table: "UserSecurityQuestions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "SecurityAnswer", "SecurityQuestionId", "UserId" },
                values: new object[] { new Guid("f15efd06-51c5-4950-94fe-365f75551dca"), new DateTime(2023, 3, 29, 9, 24, 26, 910, DateTimeKind.Utc).AddTicks(2291), new Guid("adae29ab-f889-4b27-af28-93ffb4fa4cb4"), new Guid("e41e4ff2-dc68-4539-bf12-17bc0a7e97b1"), new DateTime(2023, 3, 29, 9, 24, 26, 910, DateTimeKind.Utc).AddTicks(2292), new Guid("521864e3-c1f8-4809-88a4-c4dfde971e83"), new Guid("be452cd3-e144-4d09-8c00-daff8680214e"), "test", new Guid("700c244d-ebc9-48d9-8073-46b00d69b7d3"), new Guid("8a51ec25-b63b-42bc-b913-fd3d7526e072") });

            migrationBuilder.InsertData(
                table: "UserTags",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "Tag", "UserId" },
                values: new object[] { new Guid("23a6518a-4926-4dee-8ebb-51e49a24b6e8"), new DateTime(2023, 3, 29, 9, 24, 26, 910, DateTimeKind.Utc).AddTicks(2206), new Guid("abda35dc-f03e-4148-a6ed-31052b69ff25"), new Guid("dba310d9-31b0-475e-9ade-78145814941f"), new DateTime(2023, 3, 29, 9, 24, 26, 910, DateTimeKind.Utc).AddTicks(2207), new Guid("2ec704de-73e8-4757-8a35-d7a251f5db3e"), new Guid("cf0bcff7-e118-4051-ab49-9963231e178b"), "user-list-get", new Guid("8a51ec25-b63b-42bc-b913-fd3d7526e072") });

            migrationBuilder.CreateIndex(
                name: "IX_SecurityQuestions_Id_Question",
                table: "SecurityQuestions",
                columns: new[] { "Id", "Question" })
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_Id_DeviceId_UserId",
                table: "UserDevices",
                columns: new[] { "Id", "DeviceId", "UserId" })
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");

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
                name: "IX_UserSecurityQuestions_Id_UserId_SecurityQuestionId",
                table: "UserSecurityQuestions",
                columns: new[] { "Id", "UserId", "SecurityQuestionId" })
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");

            migrationBuilder.CreateIndex(
                name: "IX_UserSecurityQuestions_SecurityQuestionId",
                table: "UserSecurityQuestions",
                column: "SecurityQuestionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSecurityQuestions_UserId",
                table: "UserSecurityQuestions",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTags_Id_Tag_UserId",
                table: "UserTags",
                columns: new[] { "Id", "Tag", "UserId" })
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");

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
