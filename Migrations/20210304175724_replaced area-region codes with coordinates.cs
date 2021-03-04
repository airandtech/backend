using Microsoft.EntityFrameworkCore.Migrations;

namespace AirandWebAPI.Migrations
{
    public partial class replacedarearegioncodeswithcoordinates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AreaCode",
                table: "DispatchRequestInfos");

            migrationBuilder.DropColumn(
                name: "RegionCode",
                table: "DispatchRequestInfos");

            migrationBuilder.AddColumn<string>(
                name: "Cost",
                table: "DispatchRequestInfos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lat",
                table: "DispatchRequestInfos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lng",
                table: "DispatchRequestInfos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cost",
                table: "DispatchRequestInfos");

            migrationBuilder.DropColumn(
                name: "Lat",
                table: "DispatchRequestInfos");

            migrationBuilder.DropColumn(
                name: "Lng",
                table: "DispatchRequestInfos");

            migrationBuilder.AddColumn<string>(
                name: "AreaCode",
                table: "DispatchRequestInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegionCode",
                table: "DispatchRequestInfos",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
