using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace amorphie.fact.data.Migrations
{
    /// <inheritdoc />
    public partial class v5 : Migration
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
                    DeviceInfo = table.Column<string>(type: "text", nullable: true),
                    Ip = table.Column<string>(type: "text", nullable: true),
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
                name: "Idempotency",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Mode = table.Column<string>(type: "text", nullable: true),
                    Header = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Idempotency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jws",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Mode = table.Column<string>(type: "text", nullable: true),
                    Header = table.Column<string>(type: "text", nullable: true),
                    Secret = table.Column<string>(type: "text", nullable: true),
                    Algorithm = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jws", x => x.Id);
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
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    Validations = table.Column<string>(type: "text", nullable: true),
                    AvailableFlows = table.Column<string[]>(type: "text[]", nullable: true),
                    Secret = table.Column<string>(type: "text", nullable: true),
                    ReturnUrl = table.Column<string>(type: "text", nullable: true),
                    LoginUrl = table.Column<string>(type: "text", nullable: true),
                    LogoutUrl = table.Column<string>(type: "text", nullable: true),
                    Pkce = table.Column<string>(type: "text", nullable: true),
                    HeaderConfigId = table.Column<Guid>(type: "uuid", nullable: true),
                    JwsId = table.Column<Guid>(type: "uuid", nullable: true),
                    IdempotencyId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.ForeignKey(
                        name: "FK_Clients_Idempotency_IdempotencyId",
                        column: x => x.IdempotencyId,
                        principalTable: "Idempotency",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Clients_Jws_JwsId",
                        column: x => x.JwsId,
                        principalTable: "Jws",
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
                    IsArgonHash = table.Column<bool>(type: "boolean", nullable: false),
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
                name: "ClientTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    DefaultDuration = table.Column<string>(type: "text", nullable: true),
                    OverrideDuration = table.Column<bool>(type: "boolean", nullable: true),
                    PublicClaims = table.Column<string[]>(type: "text[]", nullable: true),
                    PrivateClaims = table.Column<string[]>(type: "text[]", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientTokens_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
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
                values: new object[] { new Guid("2f6254a0-2c59-4bee-8568-f06d1ac662d9"), new DateTime(2023, 5, 25, 10, 52, 40, 264, DateTimeKind.Utc).AddTicks(5153), new Guid("ac2a8157-b6bf-4bdb-869f-58d5a81aaf2b"), new Guid("35f0948d-2c65-44f6-967c-04a768c24da6"), "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", new DateTime(2023, 5, 25, 10, 52, 40, 264, DateTimeKind.Utc).AddTicks(5154), new Guid("66cd3424-7e75-4be4-bcaa-06e5390b28c8"), new Guid("43414073-e076-4a61-be12-c049626047ae") });

            migrationBuilder.InsertData(
                table: "SecurityQuestions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "Question" },
                values: new object[] { new Guid("d49473dc-384c-42d5-9277-dc4690092446"), new DateTime(2023, 5, 25, 10, 52, 40, 264, DateTimeKind.Utc).AddTicks(5185), new Guid("109069ee-9ade-4727-a4fb-43936969a47e"), new Guid("5ef250e5-fb32-47f4-b4c8-fde8d110dbae"), new DateTime(2023, 5, 25, 10, 52, 40, 264, DateTimeKind.Utc).AddTicks(5186), new Guid("8d514961-1cc0-49f6-a737-b5afb1302d72"), new Guid("ba3a375b-10ba-4fc6-ad96-c45072b2bb46"), "ilk öğretmenin adı" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CountryCode", "Number", "Prefix", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "EMail", "FirstName", "LastName", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "Reference", "Salt", "State" },
                values: new object[] { new Guid("bc1b8370-094b-49b6-900a-79c5d6261321"), 90, "1234564", 530, new DateTime(2023, 5, 25, 10, 52, 40, 264, DateTimeKind.Utc).AddTicks(4903), new Guid("34fa3670-6502-4481-8248-6fc665f9acfe"), new Guid("9867ccab-25c6-45e6-8345-572504515e64"), "test@gmail.com", "Damla", "Erhan", new DateTime(2023, 5, 25, 10, 52, 40, 264, DateTimeKind.Utc).AddTicks(4906), new Guid("e6b86949-e2a1-4e52-9a9e-7fd8b4a85d37"), new Guid("176cfc12-c616-49a0-8bdd-376044957907"), "12345678912", "fertrtretregfdgffd", "New" });

            migrationBuilder.InsertData(
                table: "UserDevices",
                columns: new[] { "Id", "ClientId", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "DeviceId", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "Status", "TokenId", "UserId" },
                values: new object[] { new Guid("48b76d29-64cb-43fb-a800-8f052ea92286"), new Guid("fa98080e-7b88-488a-9b3a-720013c9180d"), new DateTime(2023, 5, 25, 10, 52, 40, 264, DateTimeKind.Utc).AddTicks(5133), new Guid("7e76cbe3-d8de-4cfb-950a-ad2836fce842"), new Guid("a73f06d4-606f-4d75-894c-780942349f6d"), 123, new DateTime(2023, 5, 25, 10, 52, 40, 264, DateTimeKind.Utc).AddTicks(5134), new Guid("5a12671c-b700-4d65-af1a-7dc2edc2e87b"), new Guid("d50fc202-104f-401a-93cf-947b9d8a767e"), 1, new Guid("e16f41d2-f81a-4ac1-9de0-3994fd678b6b"), new Guid("bc1b8370-094b-49b6-900a-79c5d6261321") });

            migrationBuilder.InsertData(
                table: "UserPasswords",
                columns: new[] { "Id", "AccessFailedCount", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "HashedPassword", "IsArgonHash", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "MustResetPassword", "UserId" },
                values: new object[] { new Guid("0129a26e-3685-4160-a28e-cf08f28e2e7c"), null, new DateTime(2023, 5, 25, 10, 52, 40, 264, DateTimeKind.Utc).AddTicks(5225), new Guid("7534ab25-6045-479c-8823-0c37e7e719e6"), new Guid("f2c38941-5094-4d48-bf7a-dee2c677968a"), "", true, new DateTime(2023, 5, 25, 10, 52, 40, 264, DateTimeKind.Utc).AddTicks(5226), new Guid("c6fd3eae-1acd-448f-adf4-34d3046eebf6"), new Guid("790a4702-52ca-4db1-a442-f9a861080870"), null, new Guid("bc1b8370-094b-49b6-900a-79c5d6261321") });

            migrationBuilder.InsertData(
                table: "UserSecurityImages",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "SecurityImage", "UserId" },
                values: new object[] { new Guid("9c00c4c4-626b-45de-a769-7524d3f73d71"), new DateTime(2023, 5, 25, 10, 52, 40, 264, DateTimeKind.Utc).AddTicks(5170), new Guid("f2c63c2a-8897-4b16-97e4-590a0803258e"), new Guid("996c2aaa-62bc-4637-b040-9f29fdb19973"), new DateTime(2023, 5, 25, 10, 52, 40, 264, DateTimeKind.Utc).AddTicks(5171), new Guid("99c6afe0-bc06-49c2-8955-5f6a767a276d"), new Guid("292cb9a1-b06d-41f0-8f41-2d11e374f85a"), "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", new Guid("bc1b8370-094b-49b6-900a-79c5d6261321") });

            migrationBuilder.InsertData(
                table: "UserSecurityQuestions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "SecurityAnswer", "SecurityQuestionId", "UserId" },
                values: new object[] { new Guid("fa04125d-bb02-469f-99e7-aad1845210dd"), new DateTime(2023, 5, 25, 10, 52, 40, 264, DateTimeKind.Utc).AddTicks(5200), new Guid("7dd5d8dd-fc56-4ffc-991a-bb5c8dc47acd"), new Guid("8bc9dae1-8fe8-4ebe-bc42-83a8bd52af37"), new DateTime(2023, 5, 25, 10, 52, 40, 264, DateTimeKind.Utc).AddTicks(5201), new Guid("26aaf636-9e19-46f6-bf61-9640f0cb30e2"), new Guid("8bd81fe4-0629-4bdc-a365-581435ff21a2"), "test", new Guid("d49473dc-384c-42d5-9277-dc4690092446"), new Guid("bc1b8370-094b-49b6-900a-79c5d6261321") });

            migrationBuilder.InsertData(
                table: "UserTags",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "Tag", "UserId" },
                values: new object[] { new Guid("66dec1f1-18ce-4ca7-9452-7205f10fa8bc"), new DateTime(2023, 5, 25, 10, 52, 40, 264, DateTimeKind.Utc).AddTicks(5115), new Guid("f8d7f1e6-75ed-48e7-a0a3-7ae7b08476d1"), new Guid("e1f3b90f-dc54-4b54-9e66-c337a708d45c"), new DateTime(2023, 5, 25, 10, 52, 40, 264, DateTimeKind.Utc).AddTicks(5116), new Guid("a9e1a0b5-3fcb-41e2-b882-5aaf9e430ed5"), new Guid("b4bd607b-c045-4d57-9a41-d773d122b2ce"), "user-list-get", new Guid("bc1b8370-094b-49b6-900a-79c5d6261321") });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_HeaderConfigId",
                table: "Clients",
                column: "HeaderConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_IdempotencyId",
                table: "Clients",
                column: "IdempotencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_JwsId",
                table: "Clients",
                column: "JwsId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientTokens_ClientId",
                table: "ClientTokens",
                column: "ClientId");

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
                name: "ClientTokens");

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

            migrationBuilder.DropTable(
                name: "Idempotency");

            migrationBuilder.DropTable(
                name: "Jws");
        }
    }
}
