﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace amorphie.user.data.Migrations
{
    /// <inheritdoc />
    public partial class clientv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HeaderConfiguration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessToken = table.Column<string>(type: "text", nullable: true),
                    Variant = table.Column<string>(type: "text", nullable: true),
                    SessionId = table.Column<string>(type: "text", nullable: true),
                    IdempotencyId = table.Column<string>(type: "text", nullable: true),
                    DeviceInfo = table.Column<string>(type: "text", nullable: true),
                    Ip = table.Column<string>(type: "text", nullable: true),
                    Jws = table.Column<string>(type: "text", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeaderConfiguration", x => x.Id);
                });

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
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tags = table.Column<string[]>(type: "text[]", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Validations = table.Column<string>(type: "text", nullable: true),
                    AvailableFlows = table.Column<string[]>(type: "text[]", nullable: true),
                    Secret = table.Column<string>(type: "text", nullable: true),
                    ReturnUrl = table.Column<string>(type: "text", nullable: true),
                    HeaderConfigId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_HeaderConfiguration_HeaderConfigId",
                        column: x => x.HeaderConfigId,
                        principalTable: "HeaderConfiguration",
                        principalColumn: "Id");
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

            migrationBuilder.CreateTable(
                name: "Translation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Translation_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "SecurityImages",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "Image", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf" },
                values: new object[] { new Guid("a0f668cc-994e-40da-8614-cfd96ed5514a"), new DateTime(2023, 4, 5, 10, 57, 16, 37, DateTimeKind.Utc).AddTicks(995), new Guid("71a00231-2147-4b30-9f11-f03a52612cae"), new Guid("6f8dbc8c-d416-4c7b-a238-b01fa656f329"), "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", new DateTime(2023, 4, 5, 10, 57, 16, 37, DateTimeKind.Utc).AddTicks(996), new Guid("f0cc4499-7806-4e21-9720-3db352af9ab9"), new Guid("053e0d8f-a5fe-406e-9c41-dbb7bffadf6c") });

            migrationBuilder.InsertData(
                table: "SecurityQuestions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "Question" },
                values: new object[] { new Guid("922f2638-f442-413a-b881-839720a688cc"), new DateTime(2023, 4, 5, 10, 57, 16, 37, DateTimeKind.Utc).AddTicks(1025), new Guid("0fdcfe4a-bd55-4e42-8c72-9a4f382ccd0d"), new Guid("333b1fde-0aa4-4bb8-a82e-1642329fef86"), new DateTime(2023, 4, 5, 10, 57, 16, 37, DateTimeKind.Utc).AddTicks(1026), new Guid("0de5a603-4ad6-4010-8e27-2b092cee251c"), new Guid("ca216aa2-3433-4999-b7c3-756ed66241f2"), "ilk öğretmenin adı" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CountryCode", "Number", "Prefix", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "EMail", "FirstName", "LastName", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "Reference", "Salt", "State" },
                values: new object[] { new Guid("b0c7741d-06fa-441a-8efc-acf74fc30fd9"), 90, "1234564", 530, new DateTime(2023, 4, 5, 10, 57, 16, 37, DateTimeKind.Utc).AddTicks(726), new Guid("e486d6a0-2e24-46f6-ac8e-5b7f8b07fddb"), new Guid("1ad5fc0d-aefe-4873-a0e1-f3cd22ffeff2"), "test@gmail.com", "Damla", "Erhan", new DateTime(2023, 4, 5, 10, 57, 16, 37, DateTimeKind.Utc).AddTicks(730), new Guid("62c42d62-5976-4308-b956-ad0aa176b6c2"), new Guid("25a2068d-08f8-4e47-a9af-0e173c085cd9"), "12345678912", "fertrtretregfdgffd", "New" });

            migrationBuilder.InsertData(
                table: "UserDevices",
                columns: new[] { "Id", "ClientId", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "DeviceId", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "TokenId", "UserId" },
                values: new object[] { new Guid("52e4ec53-cbf2-410c-bd2a-783004ab3e9f"), new Guid("36d70b9e-84f9-41c7-a16e-5ca320d3d196"), new DateTime(2023, 4, 5, 10, 57, 16, 37, DateTimeKind.Utc).AddTicks(969), new Guid("cebccf72-a47d-4ae1-85a5-e033a33e2f5d"), new Guid("2df92af7-de82-4398-8e56-a2b8d4c4c05d"), 123, new DateTime(2023, 4, 5, 10, 57, 16, 37, DateTimeKind.Utc).AddTicks(970), new Guid("68650d64-0559-4ea7-8a19-8bcae57cc898"), new Guid("63a9dee0-76df-4e3f-bea8-a490d5308d52"), new Guid("73f95210-8f46-499d-b4c7-fe6012808e6d"), new Guid("b0c7741d-06fa-441a-8efc-acf74fc30fd9") });

            migrationBuilder.InsertData(
                table: "UserPasswords",
                columns: new[] { "Id", "AccessFailedCount", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "HashedPassword", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "MustResetPassword", "UserId" },
                values: new object[] { new Guid("98a594a2-004a-412f-9f6f-f7c34f6c482f"), null, new DateTime(2023, 4, 5, 10, 57, 16, 37, DateTimeKind.Utc).AddTicks(1068), new Guid("e3c82436-1944-4021-b5c2-8dfe93cf95bf"), new Guid("587f49f9-f9b6-4e8d-a477-f7b7d0ebcd1b"), "", new DateTime(2023, 4, 5, 10, 57, 16, 37, DateTimeKind.Utc).AddTicks(1069), new Guid("e71742e3-3398-4048-ad28-f5479c4f9842"), new Guid("f6986895-2a95-424f-876b-a7f3532ff409"), null, new Guid("b0c7741d-06fa-441a-8efc-acf74fc30fd9") });

            migrationBuilder.InsertData(
                table: "UserSecurityImages",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "SecurityImage", "UserId" },
                values: new object[] { new Guid("1968c435-a2ac-46be-9682-9f7e36106a51"), new DateTime(2023, 4, 5, 10, 57, 16, 37, DateTimeKind.Utc).AddTicks(1010), new Guid("238f3c38-3761-4fe7-8788-77793ba22a8a"), new Guid("1e8d6f3c-82a6-4f06-bf57-1ff0b453fc0c"), new DateTime(2023, 4, 5, 10, 57, 16, 37, DateTimeKind.Utc).AddTicks(1013), new Guid("5d68ddac-49ef-4fb1-bf00-38a0cd33737f"), new Guid("f3d1f99e-9883-4d86-bd1b-2244ddef1d9d"), "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", new Guid("b0c7741d-06fa-441a-8efc-acf74fc30fd9") });

            migrationBuilder.InsertData(
                table: "UserSecurityQuestions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "SecurityAnswer", "SecurityQuestionId", "UserId" },
                values: new object[] { new Guid("ffe1c77c-6026-4e84-846c-0da7ec46a663"), new DateTime(2023, 4, 5, 10, 57, 16, 37, DateTimeKind.Utc).AddTicks(1044), new Guid("af84cb18-a59a-4b1b-9019-cf828c052dc7"), new Guid("aeac7319-1604-4f7b-8c64-7f32074a9cae"), new DateTime(2023, 4, 5, 10, 57, 16, 37, DateTimeKind.Utc).AddTicks(1045), new Guid("8d4561c8-9c66-4996-8b75-6098a85a2132"), new Guid("35287fbc-c67f-47fe-b974-0a60c885a504"), "test", new Guid("922f2638-f442-413a-b881-839720a688cc"), new Guid("b0c7741d-06fa-441a-8efc-acf74fc30fd9") });

            migrationBuilder.InsertData(
                table: "UserTags",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "Tag", "UserId" },
                values: new object[] { new Guid("6bef3e12-981d-4a2c-b107-571c7ddb15d2"), new DateTime(2023, 4, 5, 10, 57, 16, 37, DateTimeKind.Utc).AddTicks(950), new Guid("d8173607-79ee-4a98-b381-68a030a2f3ad"), new Guid("9b6c15e5-e7f9-4313-af7f-4584bfd4a22e"), new DateTime(2023, 4, 5, 10, 57, 16, 37, DateTimeKind.Utc).AddTicks(951), new Guid("230d176b-c6fb-4dfd-9cb2-a481b2b80ce2"), new Guid("4b2a9be4-2011-4fa5-81cd-10a6533ea58d"), "user-list-get", new Guid("b0c7741d-06fa-441a-8efc-acf74fc30fd9") });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_HeaderConfigId",
                table: "Clients",
                column: "HeaderConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityQuestions_Id_Question",
                table: "SecurityQuestions",
                columns: new[] { "Id", "Question" })
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");

            migrationBuilder.CreateIndex(
                name: "IX_Translation_ClientId",
                table: "Translation",
                column: "ClientId");

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
                name: "Translation");

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
                name: "Clients");

            migrationBuilder.DropTable(
                name: "SecurityQuestions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "HeaderConfiguration");
        }
    }
}
