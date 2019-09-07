using Microsoft.EntityFrameworkCore.Migrations;

namespace Identity.App.Migrations
{
    public partial class AccesstoRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActionArray",
                table: "Role",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionArray",
                table: "Role");
        }
    }
}
