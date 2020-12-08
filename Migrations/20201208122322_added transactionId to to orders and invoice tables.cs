using Microsoft.EntityFrameworkCore.Migrations;

namespace AirandWebAPI.Migrations
{
    public partial class addedtransactionIdtotoordersandinvoicetables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AirandTxnId",
                table: "Invoices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AirandTxnId",
                table: "Invoices");
        }
    }
}
