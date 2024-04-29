using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace amorphie.fact.data.Migrations
{
    /// <inheritdoc />
    public partial class MigrationToProdv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeactiveDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeactiveDefinitions", x => x.Id);
                });

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
                    Code = table.Column<string>(type: "text", nullable: true),
                    Tags = table.Column<string[]>(type: "text[]", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Validations = table.Column<string>(type: "text", nullable: true),
                    Secret = table.Column<string>(type: "text", nullable: true),
                    ReturnUrl = table.Column<string>(type: "text", nullable: true),
                    LoginUrl = table.Column<string>(type: "text", nullable: true),
                    LogoutUrl = table.Column<string>(type: "text", nullable: true),
                    Pkce = table.Column<string>(type: "text", nullable: true),
                    HeaderConfigId = table.Column<Guid>(type: "uuid", nullable: true),
                    JwsId = table.Column<Guid>(type: "uuid", nullable: true),
                    IdempotencyId = table.Column<Guid>(type: "uuid", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false, computedColumnSql: "to_tsvector('english', coalesce(\"ReturnUrl\", '') || ' ' || coalesce(\"LoginUrl\", '') || ' ' || coalesce(\"LogoutUrl\", ''))", stored: true),
                    JwtSecretSalt = table.Column<string>(type: "text", nullable: true),
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
                    DeviceId = table.Column<string>(type: "text", nullable: false),
                    InstallationId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceToken = table.Column<string>(type: "text", nullable: true),
                    DeviceModel = table.Column<string>(type: "text", nullable: true),
                    DevicePlatform = table.Column<string>(type: "text", nullable: true),
                    TokenId = table.Column<Guid>(type: "uuid", nullable: true),
                    ClientId = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
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
                        principalColumn: "Id");
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
                name: "UserSmsKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SmsKey = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_UserSmsKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSmsKeys_Users_UserId",
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
                name: "ClientAudiences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientAudiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientAudiences_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientFlows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Workflow = table.Column<string>(type: "text", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: true),
                    TokenDuration = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientFlows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientFlows_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientGrantTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    GrantType = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedByBehalfOf = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientGrantTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientGrantTypes_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true),
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
                table: "Users",
                columns: new[] { "Id", "CountryCode", "Number", "Prefix", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "EMail", "FirstName", "LastName", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "Reference", "Salt", "State" },
                values: new object[] { new Guid("cc0d7bbb-19f7-48c3-9e5a-4ee61de0a6b5"), 90, "1234564", 530, new DateTime(2024, 1, 11, 15, 53, 21, 31, DateTimeKind.Utc).AddTicks(855), new Guid("28c8b3a3-182c-4917-b2f8-e7b4b79d718a"), new Guid("f830b368-5128-4660-8a8c-a758e6cdd810"), "test@gmail.com", "Damla", "Erhan", new DateTime(2024, 1, 11, 15, 53, 21, 31, DateTimeKind.Utc).AddTicks(860), new Guid("0fdbf821-a3ed-4893-a6be-a0415c565908"), new Guid("fb4af5bc-05f2-4afe-aa4e-9cf695c42a92"), "12345678912", "fertrtretregfdgffd", "New" });

            migrationBuilder.InsertData(
                table: "UserPasswords",
                columns: new[] { "Id", "AccessFailedCount", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "HashedPassword", "IsArgonHash", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "MustResetPassword", "UserId" },
                values: new object[] { new Guid("6176d7eb-cf29-4d38-9d4e-35ce9a900fb6"), null, new DateTime(2024, 1, 11, 15, 53, 21, 31, DateTimeKind.Utc).AddTicks(1534), new Guid("2bc6347c-e543-4348-8e89-7d52570a5511"), new Guid("4bbf70a6-2ab5-40a9-9d11-00d526e4731b"), "", true, new DateTime(2024, 1, 11, 15, 53, 21, 31, DateTimeKind.Utc).AddTicks(1541), new Guid("6d918189-d82c-4dd2-9e2a-e8418403cee3"), new Guid("ff9f8554-58b5-4967-938d-16bf3801a6c5"), null, new Guid("cc0d7bbb-19f7-48c3-9e5a-4ee61de0a6b5") });

            
        
            migrationBuilder.InsertData(
                table: "UserTags",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByBehalfOf", "ModifiedAt", "ModifiedBy", "ModifiedByBehalfOf", "Tag", "UserId" },
                values: new object[] { new Guid("ee04d290-04fc-4163-a45d-ad180c2730bd"), new DateTime(2024, 1, 11, 15, 53, 21, 31, DateTimeKind.Utc).AddTicks(1312), new Guid("ad278b62-d5d3-4933-b548-59ff621e8a30"), new Guid("73b57d38-0d69-4351-863f-d498478540ea"), new DateTime(2024, 1, 11, 15, 53, 21, 31, DateTimeKind.Utc).AddTicks(1315), new Guid("754b33b8-f24b-4a4e-a874-77860e5501e7"), new Guid("cb652581-6aed-4b6f-b55c-387e224058cc"), "user-list-get", new Guid("cc0d7bbb-19f7-48c3-9e5a-4ee61de0a6b5") });

            migrationBuilder.CreateIndex(
                name: "IX_ClientAudiences_ClientId",
                table: "ClientAudiences",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientFlows_ClientId",
                table: "ClientFlows",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientGrantTypes_ClientId",
                table: "ClientGrantTypes",
                column: "ClientId");

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
                name: "IX_Clients_SearchVector",
                table: "Clients",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

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
                name: "IX_UserDevices_ClientId_DeviceId",
                table: "UserDevices",
                columns: new[] { "ClientId", "DeviceId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_ClientId_DeviceId_InstallationId",
                table: "UserDevices",
                columns: new[] { "ClientId", "DeviceId", "InstallationId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_ClientId_UserId",
                table: "UserDevices",
                columns: new[] { "ClientId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_ClientId_UserId_DeviceId_InstallationId",
                table: "UserDevices",
                columns: new[] { "ClientId", "UserId", "DeviceId", "InstallationId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_DeviceId",
                table: "UserDevices",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_DeviceId_InstallationId",
                table: "UserDevices",
                columns: new[] { "DeviceId", "InstallationId" });

            /*migrationBuilder.CreateIndex(
                name: "IX_UserDevices_Id_DeviceId_UserId_ClientId",
                table: "UserDevices",
                columns: new[] { "Id", "DeviceId", "UserId", "ClientId" })
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");*/

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_UserId",
                table: "UserDevices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_UserId_DeviceId_InstallationId",
                table: "UserDevices",
                columns: new[] { "UserId", "DeviceId", "InstallationId" });

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
                name: "IX_UserSmsKeys_Id_UserId_SmsKey",
                table: "UserSmsKeys",
                columns: new[] { "Id", "UserId", "SmsKey" })
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");

            migrationBuilder.CreateIndex(
                name: "IX_UserSmsKeys_UserId",
                table: "UserSmsKeys",
                column: "UserId");

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
                name: "ClientAudiences");

            migrationBuilder.DropTable(
                name: "ClientFlows");

            migrationBuilder.DropTable(
                name: "ClientGrantTypes");

            migrationBuilder.DropTable(
                name: "ClientTokens");

            migrationBuilder.DropTable(
                name: "DeactiveDefinitions");

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
                name: "UserSmsKeys");

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
