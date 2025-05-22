using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ems_back.Repo.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDtosAndModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_FlowTemplates_FlowTemplateId",
                table: "Actions");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_CreatedBy",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_UpdatedBy",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Organizations_OrganizationId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Flows_AspNetUsers_CreatedBy",
                table: "Flows");

            migrationBuilder.DropForeignKey(
                name: "FK_Flows_AspNetUsers_UpdatedBy",
                table: "Flows");

            migrationBuilder.DropForeignKey(
                name: "FK_Flows_Events_EventId",
                table: "Flows");

            migrationBuilder.DropForeignKey(
                name: "FK_Flows_FlowTemplates_FlowTemplateId",
                table: "Flows");

            migrationBuilder.DropForeignKey(
                name: "FK_FlowTemplates_AspNetUsers_CreatedBy",
                table: "FlowTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_FlowTemplates_AspNetUsers_UpdatedBy",
                table: "FlowTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_Mail_AspNetUsers_CreatedBy",
                table: "Mail");

            migrationBuilder.DropForeignKey(
                name: "FK_Mail_AspNetUsers_UpdatedBy",
                table: "Mail");

            migrationBuilder.DropForeignKey(
                name: "FK_Organisation_User_AspNetUsers_UserId",
                table: "Organisation_User");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_AspNetUsers_CreatedBy",
                table: "Organizations");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_AspNetUsers_UpdatedBy",
                table: "Organizations");

            migrationBuilder.DropForeignKey(
                name: "FK_Triggers_FlowTemplates_FlowTemplateId",
                table: "Triggers");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Organisation_User_OrganizationId_UserId",
                table: "Organisation_User");

            migrationBuilder.DropIndex(
                name: "IX_Organisation_User_UserRole",
                table: "Organisation_User");

            //migrationBuilder.DropColumn(
            //    name: "Domain",
            //    table: "Organizations");

            migrationBuilder.RenameColumn(
                name: "Summary",
                table: "Actions",
                newName: "Description");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Triggers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Triggers",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Organizations",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "MailTemplates",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "FlowTemplates",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "FlowTemplates",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Logs",
                table: "FlowsRun",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "FlowsRun",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Flows",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Flows",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "Events",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Events",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Events",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Events",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AspNetUsers",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AspNetRoles",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AgendaEntries",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Actions",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            //migrationBuilder.CreateTable(
            //    name: "OrganizationDomain",
            //    columns: table => new
            //    {
            //        Id = table.Column<Guid>(type: "uuid", nullable: false),
            //        Domain = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
            //        OrganizationId = table.Column<Guid>(type: "uuid", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_OrganizationDomain", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_OrganizationDomain_Organizations_OrganizationId",
            //            column: x => x.OrganizationId,
            //            principalTable: "Organizations",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_User_OrganizationId",
                table: "Organisation_User",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_User_UserId_OrganizationId",
                table: "Organisation_User",
                columns: new[] { "UserId", "OrganizationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_User_UserId_OrganizationId_UserRole",
                table: "Organisation_User",
                columns: new[] { "UserId", "OrganizationId", "UserRole" },
                unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_OrganizationDomain_Domain",
            //    table: "OrganizationDomain",
            //    column: "Domain",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_OrganizationDomain_OrganizationId",
            //    table: "OrganizationDomain",
            //    column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_FlowTemplates_FlowTemplateId",
                table: "Actions",
                column: "FlowTemplateId",
                principalTable: "FlowTemplates",
                principalColumn: "FlowTemplateId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_CreatedBy",
                table: "Events",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_UpdatedBy",
                table: "Events",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Organizations_OrganizationId",
                table: "Events",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Flows_AspNetUsers_CreatedBy",
                table: "Flows",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Flows_AspNetUsers_UpdatedBy",
                table: "Flows",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Flows_Events_EventId",
                table: "Flows",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Flows_FlowTemplates_FlowTemplateId",
                table: "Flows",
                column: "FlowTemplateId",
                principalTable: "FlowTemplates",
                principalColumn: "FlowTemplateId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowTemplates_AspNetUsers_CreatedBy",
                table: "FlowTemplates",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowTemplates_AspNetUsers_UpdatedBy",
                table: "FlowTemplates",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Mail_AspNetUsers_CreatedBy",
                table: "Mail",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Mail_AspNetUsers_UpdatedBy",
                table: "Mail",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Organisation_User_AspNetUsers_UserId",
                table: "Organisation_User",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_AspNetUsers_CreatedBy",
                table: "Organizations",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_AspNetUsers_UpdatedBy",
                table: "Organizations",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Triggers_FlowTemplates_FlowTemplateId",
                table: "Triggers",
                column: "FlowTemplateId",
                principalTable: "FlowTemplates",
                principalColumn: "FlowTemplateId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_FlowTemplates_FlowTemplateId",
                table: "Actions");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_CreatedBy",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_UpdatedBy",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Organizations_OrganizationId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Flows_AspNetUsers_CreatedBy",
                table: "Flows");

            migrationBuilder.DropForeignKey(
                name: "FK_Flows_AspNetUsers_UpdatedBy",
                table: "Flows");

            migrationBuilder.DropForeignKey(
                name: "FK_Flows_Events_EventId",
                table: "Flows");

            migrationBuilder.DropForeignKey(
                name: "FK_Flows_FlowTemplates_FlowTemplateId",
                table: "Flows");

            migrationBuilder.DropForeignKey(
                name: "FK_FlowTemplates_AspNetUsers_CreatedBy",
                table: "FlowTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_FlowTemplates_AspNetUsers_UpdatedBy",
                table: "FlowTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_Mail_AspNetUsers_CreatedBy",
                table: "Mail");

            migrationBuilder.DropForeignKey(
                name: "FK_Mail_AspNetUsers_UpdatedBy",
                table: "Mail");

            migrationBuilder.DropForeignKey(
                name: "FK_Organisation_User_AspNetUsers_UserId",
                table: "Organisation_User");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_AspNetUsers_CreatedBy",
                table: "Organizations");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_AspNetUsers_UpdatedBy",
                table: "Organizations");

            migrationBuilder.DropForeignKey(
                name: "FK_Triggers_FlowTemplates_FlowTemplateId",
                table: "Triggers");

            migrationBuilder.DropTable(
                name: "OrganizationDomain");

            migrationBuilder.DropIndex(
                name: "IX_Organisation_User_OrganizationId",
                table: "Organisation_User");

            migrationBuilder.DropIndex(
                name: "IX_Organisation_User_UserId_OrganizationId",
                table: "Organisation_User");

            migrationBuilder.DropIndex(
                name: "IX_Organisation_User_UserId_OrganizationId_UserRole",
                table: "Organisation_User");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Actions",
                newName: "Summary");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Triggers",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Triggers",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Organizations",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "Organizations",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "MailTemplates",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "FlowTemplates",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "FlowTemplates",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Logs",
                table: "FlowsRun",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "FlowsRun",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Flows",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Flows",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "Events",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Events",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Events",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Events",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AspNetUsers",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AspNetRoles",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AgendaEntries",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Actions",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    UploadedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: true),
                    Type = table.Column<int>(type: "integer", maxLength: 50, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_AspNetUsers_UploadedBy",
                        column: x => x.UploadedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Files_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_User_OrganizationId_UserId",
                table: "Organisation_User",
                columns: new[] { "OrganizationId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_User_UserRole",
                table: "Organisation_User",
                column: "UserRole");

            migrationBuilder.CreateIndex(
                name: "IX_Files_EventId",
                table: "Files",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_UploadedBy",
                table: "Files",
                column: "UploadedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_FlowTemplates_FlowTemplateId",
                table: "Actions",
                column: "FlowTemplateId",
                principalTable: "FlowTemplates",
                principalColumn: "FlowTemplateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_CreatedBy",
                table: "Events",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_UpdatedBy",
                table: "Events",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Organizations_OrganizationId",
                table: "Events",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Flows_AspNetUsers_CreatedBy",
                table: "Flows",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Flows_AspNetUsers_UpdatedBy",
                table: "Flows",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Flows_Events_EventId",
                table: "Flows",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Flows_FlowTemplates_FlowTemplateId",
                table: "Flows",
                column: "FlowTemplateId",
                principalTable: "FlowTemplates",
                principalColumn: "FlowTemplateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowTemplates_AspNetUsers_CreatedBy",
                table: "FlowTemplates",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowTemplates_AspNetUsers_UpdatedBy",
                table: "FlowTemplates",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Organisation_User_AspNetUsers_UserId",
                table: "Organisation_User",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_AspNetUsers_CreatedBy",
                table: "Organizations",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_AspNetUsers_UpdatedBy",
                table: "Organizations",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Triggers_FlowTemplates_FlowTemplateId",
                table: "Triggers",
                column: "FlowTemplateId",
                principalTable: "FlowTemplates",
                principalColumn: "FlowTemplateId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
