using Microsoft.EntityFrameworkCore.Migrations;

namespace Mirza.Web.Migrations
{
    public partial class RemoveWorkLogFromTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkLogs_Teams_TeamId",
                table: "WorkLogs");

            migrationBuilder.DropIndex(
                name: "IX_WorkLogs_TeamId",
                table: "WorkLogs");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "WorkLogs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "WorkLogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_TeamId",
                table: "WorkLogs",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkLogs_Teams_TeamId",
                table: "WorkLogs",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
