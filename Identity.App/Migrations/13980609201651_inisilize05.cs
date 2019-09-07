using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Identity.App.Migrations
{
    public partial class inisilize05 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "UserUsedPassword",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "UserToken",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "UserRole",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "UserLogin",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "UserClaim",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "User",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "RoleClaim",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "Role",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Guid",
                table: "UserUsedPassword");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "UserToken");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "UserLogin");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "UserClaim");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "RoleClaim");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Role");
        }
    }
}
