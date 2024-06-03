using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWN.Migrations
{
    public partial class updatePayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Users_AccountId",
                schema: "AwnSc",
                table: "Payment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payment",
                schema: "AwnSc",
                table: "Payment");

            migrationBuilder.RenameTable(
                name: "Payment",
                schema: "AwnSc",
                newName: "payments",
                newSchema: "AwnSc");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_AccountId",
                schema: "AwnSc",
                table: "payments",
                newName: "IX_payments_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_payments",
                schema: "AwnSc",
                table: "payments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_payments_Users_AccountId",
                schema: "AwnSc",
                table: "payments",
                column: "AccountId",
                principalSchema: "AwnSc",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_payments_Users_AccountId",
                schema: "AwnSc",
                table: "payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_payments",
                schema: "AwnSc",
                table: "payments");

            migrationBuilder.RenameTable(
                name: "payments",
                schema: "AwnSc",
                newName: "Payment",
                newSchema: "AwnSc");

            migrationBuilder.RenameIndex(
                name: "IX_payments_AccountId",
                schema: "AwnSc",
                table: "Payment",
                newName: "IX_Payment_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payment",
                schema: "AwnSc",
                table: "Payment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Users_AccountId",
                schema: "AwnSc",
                table: "Payment",
                column: "AccountId",
                principalSchema: "AwnSc",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
