using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWN.Migrations
{
    public partial class upadatePaymentWithDonateCase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DonateCaseId",
                schema: "AwnSc",
                table: "payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_payments_DonateCaseId",
                schema: "AwnSc",
                table: "payments",
                column: "DonateCaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_payments_donateCases_DonateCaseId",
                schema: "AwnSc",
                table: "payments",
                column: "DonateCaseId",
                principalSchema: "AwnSc",
                principalTable: "donateCases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_payments_donateCases_DonateCaseId",
                schema: "AwnSc",
                table: "payments");

            migrationBuilder.DropIndex(
                name: "IX_payments_DonateCaseId",
                schema: "AwnSc",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "DonateCaseId",
                schema: "AwnSc",
                table: "payments");
        }
    }
}
