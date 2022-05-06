using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PaymentControlAPI.Migrations
{
    public partial class initialmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblEmailLog",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "100000, 1"),
                    ToEmail = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    Subject = table.Column<string>(maxLength: 200, nullable: false),
                    Body = table.Column<string>(unicode: false, maxLength: 5000, nullable: false),
                    RequestId = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    Status = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Signature = table.Column<string>(unicode: false, maxLength: 500, nullable: false),
                    DropTimestamp = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblEmailLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblRequestAndReponse",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "10000, 1"),
                    RequestType = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    RequestPayload = table.Column<string>(maxLength: 5000, nullable: false),
                    RequestTimestamp = table.Column<DateTime>(type: "datetime", nullable: false),
                    Response = table.Column<string>(maxLength: 2147483647, nullable: true),
                    ResponseTimestamp = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblRequestAndReponse", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblEmailLog");

            migrationBuilder.DropTable(
                name: "tblRequestAndReponse");
        }
    }
}
