using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWN.Migrations
{
    public partial class CreateTables2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notifications",
                schema: "AwnSc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    TimesTamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "suggestions",
                schema: "AwnSc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sort = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suggestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountNotification",
                schema: "AwnSc",
                columns: table => new
                {
                    accountsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    notificationsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountNotification", x => new { x.accountsId, x.notificationsId });
                    table.ForeignKey(
                        name: "FK_AccountNotification_notifications_notificationsId",
                        column: x => x.notificationsId,
                        principalSchema: "AwnSc",
                        principalTable: "notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountNotification_Users_accountsId",
                        column: x => x.accountsId,
                        principalSchema: "AwnSc",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountSuggestion",
                schema: "AwnSc",
                columns: table => new
                {
                    AccountsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    suggestionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountSuggestion", x => new { x.AccountsId, x.suggestionsId });
                    table.ForeignKey(
                        name: "FK_AccountSuggestion_suggestions_suggestionsId",
                        column: x => x.suggestionsId,
                        principalSchema: "AwnSc",
                        principalTable: "suggestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountSuggestion_Users_AccountsId",
                        column: x => x.AccountsId,
                        principalSchema: "AwnSc",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountNotification_notificationsId",
                schema: "AwnSc",
                table: "AccountNotification",
                column: "notificationsId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSuggestion_suggestionsId",
                schema: "AwnSc",
                table: "AccountSuggestion",
                column: "suggestionsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountNotification",
                schema: "AwnSc");

            migrationBuilder.DropTable(
                name: "AccountSuggestion",
                schema: "AwnSc");

            migrationBuilder.DropTable(
                name: "notifications",
                schema: "AwnSc");

            migrationBuilder.DropTable(
                name: "suggestions",
                schema: "AwnSc");
        }
    }
}
