using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mirza.Web.Migrations
{
    public partial class AddLogDateFieldToWorkLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LogDate",
                table: "WorkLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 6, 14, 41, 0, 100, DateTimeKind.Local).AddTicks(4390));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogDate",
                table: "WorkLogs");
        }
    }
}
