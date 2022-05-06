using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PaymentControlAPI.Migrations
{
    public partial class myFisrtDBChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "emailDatetime",
                table: "tblEmailLog",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "emailStatus",
                table: "tblEmailLog",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "emailDatetime",
                table: "tblEmailLog");

            migrationBuilder.DropColumn(
                name: "emailStatus",
                table: "tblEmailLog");
        }
    }
}
