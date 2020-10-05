using Microsoft.EntityFrameworkCore.Migrations;

namespace AirandWebAPI.Migrations
{
    public partial class addedrequestoridentifertoorderstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestorIdentifier",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestorIdentifier",
                table: "Orders");
        }
    }
}
