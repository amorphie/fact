using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amorphie.fact.data.Migrations
{
    /// <inheritdoc />
    public partial class CanCreateLoginUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanCreateLoginUrl",
                table: "Clients",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string[]>(
                name: "CreateLoginUrlClients",
                table: "Clients",
                type: "text[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanCreateLoginUrl",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "CreateLoginUrlClients",
                table: "Clients");
        }
    }
}
