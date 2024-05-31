using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWN.Migrations
{
    public partial class CreateTables3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                schema: "AwnSc",
                table: "suggestions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                schema: "AwnSc",
                table: "requestJoins",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                schema: "AwnSc",
                table: "requestJoins",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Payment",
                schema: "AwnSc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    TimesTamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    PaymentMethod = table.Column<int>(type: "int", nullable: true),
                    AccountId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_Users_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "AwnSc",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_requestJoins_AccountId",
                schema: "AwnSc",
                table: "requestJoins",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_AccountId",
                schema: "AwnSc",
                table: "Payment",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_requestJoins_Users_AccountId",
                schema: "AwnSc",
                table: "requestJoins",
                column: "AccountId",
                principalSchema: "AwnSc",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_requestJoins_Users_AccountId",
                schema: "AwnSc",
                table: "requestJoins");

            migrationBuilder.DropTable(
                name: "Payment",
                schema: "AwnSc");

            migrationBuilder.DropIndex(
                name: "IX_requestJoins_AccountId",
                schema: "AwnSc",
                table: "requestJoins");

            migrationBuilder.DropColumn(
                name: "IsAccepted",
                schema: "AwnSc",
                table: "suggestions");

            migrationBuilder.DropColumn(
                name: "AccountId",
                schema: "AwnSc",
                table: "requestJoins");

            migrationBuilder.DropColumn(
                name: "IsAccepted",
                schema: "AwnSc",
                table: "requestJoins");
        }
    }
}
