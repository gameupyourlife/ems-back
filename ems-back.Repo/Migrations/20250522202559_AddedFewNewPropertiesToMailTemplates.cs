using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ems_back.Repo.Migrations
{
    /// <inheritdoc />
    public partial class AddedFewNewPropertiesToMailTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MainRunId",
                table: "MailRun",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "MailId",
                table: "Mail",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "MailTemplates",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MailTemplates",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "MailTemplates",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "MailTemplates",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid[]>(
                name: "Recipients",
                table: "MailTemplates",
                type: "uuid[]",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledFor",
                table: "MailTemplates",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "MailTemplates",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "MailTemplates",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "sendToAllParticipants",
                table: "MailTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid[]>(
                name: "Recipients",
                table: "Mail",
                type: "uuid[]",
                nullable: true,
                oldClrType: typeof(Guid[]),
                oldType: "uuid[]");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Mail",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Mail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUserCreated",
                table: "Mail",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "sendToAllParticipants",
                table: "Mail",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_MailTemplates_CreatedBy",
                table: "MailTemplates",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MailTemplates_UpdatedBy",
                table: "MailTemplates",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_MailTemplates_AspNetUsers_CreatedBy",
                table: "MailTemplates",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MailTemplates_AspNetUsers_UpdatedBy",
                table: "MailTemplates",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MailTemplates_AspNetUsers_CreatedBy",
                table: "MailTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_MailTemplates_AspNetUsers_UpdatedBy",
                table: "MailTemplates");

            migrationBuilder.DropIndex(
                name: "IX_MailTemplates_CreatedBy",
                table: "MailTemplates");

            migrationBuilder.DropIndex(
                name: "IX_MailTemplates_UpdatedBy",
                table: "MailTemplates");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "MailTemplates");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "MailTemplates");

            migrationBuilder.DropColumn(
                name: "Recipients",
                table: "MailTemplates");

            migrationBuilder.DropColumn(
                name: "ScheduledFor",
                table: "MailTemplates");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MailTemplates");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "MailTemplates");

            migrationBuilder.DropColumn(
                name: "sendToAllParticipants",
                table: "MailTemplates");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Mail");

            migrationBuilder.DropColumn(
                name: "IsUserCreated",
                table: "Mail");

            migrationBuilder.DropColumn(
                name: "sendToAllParticipants",
                table: "Mail");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "MailRun",
                newName: "MainRunId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Mail",
                newName: "MailId");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "MailTemplates",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MailTemplates",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<Guid[]>(
                name: "Recipients",
                table: "Mail",
                type: "uuid[]",
                nullable: false,
                defaultValue: new Guid[0],
                oldClrType: typeof(Guid[]),
                oldType: "uuid[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Mail",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
