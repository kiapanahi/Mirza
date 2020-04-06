using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mirza.Web.Migrations
{
    public partial class ChangeDefaultValueOfWorkLogLogDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LogDate",
                table: "WorkLogs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2020, 4, 6, 14, 41, 0, 100, DateTimeKind.Local).AddTicks(4390));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LogDate",
                table: "WorkLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2020, 4, 6, 14, 41, 0, 100, DateTimeKind.Local).AddTicks(4390),
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
