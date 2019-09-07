using Microsoft.EntityFrameworkCore.Migrations;

namespace Identity.App.Migrations
{
    public partial class inisials03 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Enable",
                table: "Role",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Role",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Enable",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Role");
        }
    }
}
