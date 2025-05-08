using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ems_back.Repo.Migrations
{
    /// <inheritdoc />
    public partial class FixedDoubleRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_FlowTemplates_FlowTemplateId1",
                table: "Actions");

            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Flows_FlowId1",
                table: "Actions");

            migrationBuilder.DropForeignKey(
                name: "FK_Flows_FlowTemplates_FlowTemplateId1",
                table: "Flows");

            migrationBuilder.DropForeignKey(
                name: "FK_FlowTemplates_Organizations_OrganizationId1",
                table: "FlowTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_MailTemplates_Organizations_OrganizationId1",
                table: "MailTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_Triggers_FlowTemplates_FlowTemplateId1",
                table: "Triggers");

            migrationBuilder.DropForeignKey(
                name: "FK_Triggers_Flows_FlowId1",
                table: "Triggers");

            migrationBuilder.DropIndex(
                name: "IX_Triggers_FlowId1",
                table: "Triggers");

            migrationBuilder.DropIndex(
                name: "IX_Triggers_FlowTemplateId1",
                table: "Triggers");

            migrationBuilder.DropIndex(
                name: "IX_MailTemplates_OrganizationId1",
                table: "MailTemplates");

            migrationBuilder.DropIndex(
                name: "IX_FlowTemplates_OrganizationId1",
                table: "FlowTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Flows_FlowTemplateId1",
                table: "Flows");

            migrationBuilder.DropIndex(
                name: "IX_Actions_FlowId1",
                table: "Actions");

            migrationBuilder.DropIndex(
                name: "IX_Actions_FlowTemplateId1",
                table: "Actions");

            migrationBuilder.DropColumn(
                name: "FlowId1",
                table: "Triggers");

            migrationBuilder.DropColumn(
                name: "FlowTemplateId1",
                table: "Triggers");

            migrationBuilder.DropColumn(
                name: "OrganizationId1",
                table: "MailTemplates");

            migrationBuilder.DropColumn(
                name: "OrganizationId1",
                table: "FlowTemplates");

            migrationBuilder.DropColumn(
                name: "FlowTemplateId1",
                table: "Flows");

            migrationBuilder.DropColumn(
                name: "Attended",
                table: "EventAttendees");

            migrationBuilder.DropColumn(
                name: "FlowId1",
                table: "Actions");

            migrationBuilder.DropColumn(
                name: "FlowTemplateId1",
                table: "Actions");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "EventAttendees",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "EventAttendees");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "FlowId1",
                table: "Triggers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FlowTemplateId1",
                table: "Triggers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId1",
                table: "MailTemplates",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId1",
                table: "FlowTemplates",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "FlowTemplateId1",
                table: "Flows",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Attended",
                table: "EventAttendees",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "FlowId1",
                table: "Actions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FlowTemplateId1",
                table: "Actions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Triggers_FlowId1",
                table: "Triggers",
                column: "FlowId1");

            migrationBuilder.CreateIndex(
                name: "IX_Triggers_FlowTemplateId1",
                table: "Triggers",
                column: "FlowTemplateId1");

            migrationBuilder.CreateIndex(
                name: "IX_MailTemplates_OrganizationId1",
                table: "MailTemplates",
                column: "OrganizationId1");

            migrationBuilder.CreateIndex(
                name: "IX_FlowTemplates_OrganizationId1",
                table: "FlowTemplates",
                column: "OrganizationId1");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_FlowTemplateId1",
                table: "Flows",
                column: "FlowTemplateId1");

            migrationBuilder.CreateIndex(
                name: "IX_Actions_FlowId1",
                table: "Actions",
                column: "FlowId1");

            migrationBuilder.CreateIndex(
                name: "IX_Actions_FlowTemplateId1",
                table: "Actions",
                column: "FlowTemplateId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_FlowTemplates_FlowTemplateId1",
                table: "Actions",
                column: "FlowTemplateId1",
                principalTable: "FlowTemplates",
                principalColumn: "FlowTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Flows_FlowId1",
                table: "Actions",
                column: "FlowId1",
                principalTable: "Flows",
                principalColumn: "FlowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flows_FlowTemplates_FlowTemplateId1",
                table: "Flows",
                column: "FlowTemplateId1",
                principalTable: "FlowTemplates",
                principalColumn: "FlowTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowTemplates_Organizations_OrganizationId1",
                table: "FlowTemplates",
                column: "OrganizationId1",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MailTemplates_Organizations_OrganizationId1",
                table: "MailTemplates",
                column: "OrganizationId1",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Triggers_FlowTemplates_FlowTemplateId1",
                table: "Triggers",
                column: "FlowTemplateId1",
                principalTable: "FlowTemplates",
                principalColumn: "FlowTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Triggers_Flows_FlowId1",
                table: "Triggers",
                column: "FlowId1",
                principalTable: "Flows",
                principalColumn: "FlowId");
        }
    }
}
