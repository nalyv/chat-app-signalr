using Microsoft.EntityFrameworkCore.Migrations;

namespace chat_application.Migrations
{
    public partial class messagesEdited : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Messages",
                newName: "SenderName");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverName",
                table: "Messages",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverName",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "SenderName",
                table: "Messages",
                newName: "Username");
        }
    }
}
