using Microsoft.EntityFrameworkCore.Migrations;

namespace Evect.Migrations
{
    public partial class apikey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiKey",
                table: "MobileUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiKey",
                table: "MobileUsers");
        }
    }
}
