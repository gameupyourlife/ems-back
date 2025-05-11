using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ems_back.Repo.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTriggerAndActionsAttributesSummaryAndName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Mail");

            migrationBuilder.DropColumn(
                name: "IsUserCreated",
                table: "Mail");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Triggers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Triggers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Mail",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Mail",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string[]>(
                name: "Recipients",
                table: "Mail",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledFor",
                table: "Mail",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Mail",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "Mail",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Actions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Actions",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mail_CreatedBy",
                table: "Mail",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Mail_UpdatedBy",
                table: "Mail",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Mail_AspNetUsers_CreatedBy",
                table: "Mail",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mail_AspNetUsers_UpdatedBy",
                table: "Mail",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mail_AspNetUsers_CreatedBy",
                table: "Mail");

            migrationBuilder.DropForeignKey(
                name: "FK_Mail_AspNetUsers_UpdatedBy",
                table: "Mail");

            migrationBuilder.DropIndex(
                name: "IX_Mail_CreatedBy",
                table: "Mail");

            migrationBuilder.DropIndex(
                name: "IX_Mail_UpdatedBy",
                table: "Mail");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Triggers");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Triggers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Mail");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Mail");

            migrationBuilder.DropColumn(
                name: "Recipients",
                table: "Mail");

            migrationBuilder.DropColumn(
                name: "ScheduledFor",
                table: "Mail");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Mail");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Mail");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Actions");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Actions");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Mail",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsUserCreated",
                table: "Mail",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
