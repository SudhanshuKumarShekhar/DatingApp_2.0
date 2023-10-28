using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatingApp.Migrations
{
    /// <inheritdoc />
    public partial class MessageModelAddedupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessagesSent_Users_RecipientId",
                table: "MessagesSent");

            migrationBuilder.DropForeignKey(
                name: "FK_MessagesSent_Users_SenderId",
                table: "MessagesSent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessagesSent",
                table: "MessagesSent");

            migrationBuilder.RenameTable(
                name: "MessagesSent",
                newName: "Messages");

            migrationBuilder.RenameIndex(
                name: "IX_MessagesSent_SenderId",
                table: "Messages",
                newName: "IX_Messages_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_MessagesSent_RecipientId",
                table: "Messages",
                newName: "IX_Messages_RecipientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Messages",
                table: "Messages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_RecipientId",
                table: "Messages",
                column: "RecipientId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_RecipientId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_SenderId",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Messages",
                table: "Messages");

            migrationBuilder.RenameTable(
                name: "Messages",
                newName: "MessagesSent");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_SenderId",
                table: "MessagesSent",
                newName: "IX_MessagesSent_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_RecipientId",
                table: "MessagesSent",
                newName: "IX_MessagesSent_RecipientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessagesSent",
                table: "MessagesSent",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MessagesSent_Users_RecipientId",
                table: "MessagesSent",
                column: "RecipientId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MessagesSent_Users_SenderId",
                table: "MessagesSent",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
