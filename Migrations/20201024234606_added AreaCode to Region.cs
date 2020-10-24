using Microsoft.EntityFrameworkCore.Migrations;

namespace AirandWebAPI.Migrations
{
    public partial class addedAreaCodetoRegion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AreaCode",
                table: "Regions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AreaCode",
                table: "DispatchRequestInfos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AreaCode",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "AreaCode",
                table: "DispatchRequestInfos");
        }
    }
}
