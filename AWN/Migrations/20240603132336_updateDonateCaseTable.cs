using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWN.Migrations
{
    public partial class updateDonateCaseTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ExcessAmount",
                schema: "AwnSc",
                table: "donateCases",
                type: "float",
                nullable: false,
                computedColumnSql: "CASE WHEN [CurrentAmount] <= [TargetAmount] THEN 0 ELSE [CurrentAmount] - [TargetAmount] END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcessAmount",
                schema: "AwnSc",
                table: "donateCases");
        }
    }
}
