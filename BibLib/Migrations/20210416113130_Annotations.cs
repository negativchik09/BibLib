using Microsoft.EntityFrameworkCore.Migrations;

namespace BibLib.Migrations
{
    public partial class Annotations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Annotation",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Annotation",
                table: "Books");
        }
    }
}
