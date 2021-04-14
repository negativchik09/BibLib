using Microsoft.EntityFrameworkCore.Migrations;

namespace BibLib.Migrations
{
    public partial class SecretQuestionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SecretQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SecretQuestions",
                table: "SecretQuestions",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SecretQuestions",
                table: "SecretQuestions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SecretQuestions");
        }
    }
}
