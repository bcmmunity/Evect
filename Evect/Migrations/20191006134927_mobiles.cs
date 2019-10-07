using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Evect.Migrations
{
    public partial class mobiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "time",
                table: "TimeToJoinToEvents",
                newName: "Time");

            migrationBuilder.CreateTable(
                name: "MobileUsers",
                columns: table => new
                {
                    MobileUserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: true),
                    EmailCode = table.Column<string>(nullable: true),
                    TelegramId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileUsers", x => x.MobileUserId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MobileUsers");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "TimeToJoinToEvents",
                newName: "time");
        }
    }
}
