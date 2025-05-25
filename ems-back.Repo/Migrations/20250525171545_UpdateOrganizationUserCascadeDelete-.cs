using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ems_back.Repo.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrganizationUserCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organisation_User_AspNetUsers_UserId",
                table: "Organisation_User");

            migrationBuilder.RenameColumn(
                name: "sendToAllParticipants",
                table: "MailTemplates",
                newName: "SendToAllParticipants");

            migrationBuilder.AddForeignKey(
                name: "FK_Organisation_User_AspNetUsers_UserId",
                table: "Organisation_User",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organisation_User_AspNetUsers_UserId",
                table: "Organisation_User");

            migrationBuilder.RenameColumn(
                name: "SendToAllParticipants",
                table: "MailTemplates",
                newName: "sendToAllParticipants");

            migrationBuilder.AddForeignKey(
                name: "FK_Organisation_User_AspNetUsers_UserId",
                table: "Organisation_User",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
