using Microsoft.EntityFrameworkCore.Migrations;

namespace AirandWebAPI.Migrations
{
    public partial class dispatchmanagers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DispatchManager_Companies_CompanyId",
                table: "DispatchManager");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DispatchManager",
                table: "DispatchManager");

            migrationBuilder.RenameTable(
                name: "DispatchManager",
                newName: "DispatchManagers");

            migrationBuilder.RenameIndex(
                name: "IX_DispatchManager_CompanyId",
                table: "DispatchManagers",
                newName: "IX_DispatchManagers_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DispatchManagers",
                table: "DispatchManagers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DispatchManagers_Companies_CompanyId",
                table: "DispatchManagers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DispatchManagers_Companies_CompanyId",
                table: "DispatchManagers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DispatchManagers",
                table: "DispatchManagers");

            migrationBuilder.RenameTable(
                name: "DispatchManagers",
                newName: "DispatchManager");

            migrationBuilder.RenameIndex(
                name: "IX_DispatchManagers_CompanyId",
                table: "DispatchManager",
                newName: "IX_DispatchManager_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DispatchManager",
                table: "DispatchManager",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DispatchManager_Companies_CompanyId",
                table: "DispatchManager",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
