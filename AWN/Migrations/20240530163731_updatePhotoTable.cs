using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWN.Migrations
{
    public partial class updatePhotoTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_photos",
                schema: "AwnSc",
                table: "photos");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Photo",
                schema: "AwnSc",
                table: "photos",
                type: "varbinary(max)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(900)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                schema: "AwnSc",
                table: "photos",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_photos",
                schema: "AwnSc",
                table: "photos",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_photos_DonateCaseId",
                schema: "AwnSc",
                table: "photos",
                column: "DonateCaseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_photos",
                schema: "AwnSc",
                table: "photos");

            migrationBuilder.DropIndex(
                name: "IX_photos_DonateCaseId",
                schema: "AwnSc",
                table: "photos");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "AwnSc",
                table: "photos");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Photo",
                schema: "AwnSc",
                table: "photos",
                type: "varbinary(900)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_photos",
                schema: "AwnSc",
                table: "photos",
                columns: new[] { "DonateCaseId", "Photo" });
        }
    }
}
