using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amorphie.fact.data.Migrations
{
    /// <inheritdoc />
    public partial class ClientLoginFlowName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoginWorkflowName",
                table: "Clients",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoginWorkflowName",
                table: "Clients");
        }
    }
}
