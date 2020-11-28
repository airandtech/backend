using Microsoft.EntityFrameworkCore.Migrations;

namespace AirandWebAPI.Migrations
{
    public partial class descriptiontypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Desccription",
                table: "Riders");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Riders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Riders");

            migrationBuilder.AddColumn<string>(
                name: "Desccription",
                table: "Riders",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
