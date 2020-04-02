using Microsoft.EntityFrameworkCore.Migrations;

namespace Mirza.Web.Migrations
{
    public partial class FixAccessKeyExpirationTypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Expriation",
                table: "AccessKeys",
                newName: "Expiration");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Expiration",
                table: "AccessKeys",
                newName: "Expriation");
        }
    }
}
