using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWN.Migrations
{
    public partial class UpdateSuggestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                schema: "AwnSc",
                table: "suggestions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_suggestions_AccountId",
                schema: "AwnSc",
                table: "suggestions",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_suggestions_Users_AccountId",
                schema: "AwnSc",
                table: "suggestions",
                column: "AccountId",
                principalSchema: "AwnSc",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_suggestions_Users_AccountId",
                schema: "AwnSc",
                table: "suggestions");

            migrationBuilder.DropIndex(
                name: "IX_suggestions_AccountId",
                schema: "AwnSc",
                table: "suggestions");

            migrationBuilder.DropColumn(
                name: "AccountId",
                schema: "AwnSc",
                table: "suggestions");

        }
    }
}
