using Microsoft.EntityFrameworkCore.Migrations;

namespace Evect.Migrations
{
    public partial class new1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_ContactsBooks_Users_UserId", "ContactsBooks", "u0641156_evect");
            
            migrationBuilder.AddForeignKey(
                name: "FK_ContactsBooks_Users_UserId",
                table: "ContactsBooks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
