using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWN.Migrations
{
    public partial class test1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Support_Users_AccountId",
                schema: "AwnSc",
                table: "Support");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Support",
                schema: "AwnSc",
                table: "Support");

            migrationBuilder.RenameTable(
                name: "Support",
                schema: "AwnSc",
                newName: "supports",
                newSchema: "AwnSc");

            migrationBuilder.RenameIndex(
                name: "IX_Support_AccountId",
                schema: "AwnSc",
                table: "supports",
                newName: "IX_supports_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_supports",
                schema: "AwnSc",
                table: "supports",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_supports_Users_AccountId",
                schema: "AwnSc",
                table: "supports",
                column: "AccountId",
                principalSchema: "AwnSc",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_supports_Users_AccountId",
                schema: "AwnSc",
                table: "supports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_supports",
                schema: "AwnSc",
                table: "supports");

            migrationBuilder.RenameTable(
                name: "supports",
                schema: "AwnSc",
                newName: "Support",
                newSchema: "AwnSc");

            migrationBuilder.RenameIndex(
                name: "IX_supports_AccountId",
                schema: "AwnSc",
                table: "Support",
                newName: "IX_Support_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Support",
                schema: "AwnSc",
                table: "Support",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Support_Users_AccountId",
                schema: "AwnSc",
                table: "Support",
                column: "AccountId",
                principalSchema: "AwnSc",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
