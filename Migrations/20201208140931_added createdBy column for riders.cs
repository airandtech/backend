using Microsoft.EntityFrameworkCore.Migrations;

namespace AirandWebAPI.Migrations
{
    public partial class addedcreatedBycolumnforriders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Riders",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Riders");
        }
    }
}
