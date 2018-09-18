using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PPI.API.Migrations
{
    public partial class AddPasswordSaltField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "passwordSalt",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "passwordSalt",
                table: "Users");
        }
    }
}
