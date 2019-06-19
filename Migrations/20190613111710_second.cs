using Microsoft.EntityFrameworkCore.Migrations;

namespace chat_application.Migrations
{
    public partial class second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Connections",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "isActive",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserName);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Connections_UserName",
                table: "Connections",
                column: "UserName");

            migrationBuilder.AddForeignKey(
                name: "FK_Connections_User_UserName",
                table: "Connections",
                column: "UserName",
                principalTable: "User",
                principalColumn: "UserName",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connections_User_UserName",
                table: "Connections");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropIndex(
                name: "IX_Connections_UserName",
                table: "Connections");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Connections");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "AspNetUsers");
        }
    }
}
