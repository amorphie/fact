using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amorphie.fact.data.Migrations
{
    /// <inheritdoc />
    public partial class SecurityQuestionDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "SecurityQuestions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionTr",
                table: "SecurityQuestions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "SecurityQuestions");

            migrationBuilder.DropColumn(
                name: "DescriptionTr",
                table: "SecurityQuestions");
        }
    }
}
