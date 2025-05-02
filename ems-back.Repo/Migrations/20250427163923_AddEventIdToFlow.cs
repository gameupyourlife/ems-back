using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ems_back.Repo.Migrations
{
    /// <inheritdoc />
    public partial class AddEventIdToFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Flows_FlowId",
                table: "Actions");

            migrationBuilder.DropForeignKey(
                name: "FK_FlowsRun_Flows_FlowId",
                table: "FlowsRun");

            migrationBuilder.DropForeignKey(
                name: "FK_Triggers_Flows_FlowId",
                table: "Triggers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Flows",
                table: "Flows");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Flows");

            migrationBuilder.AddColumn<Guid>(
                name: "FlowId",
                table: "Flows",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "EventId",
                table: "Flows",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "multipleRuns",
                table: "Flows",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "stillPending",
                table: "Flows",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Flows",
                table: "Flows",
                column: "FlowId");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_EventId",
                table: "Flows",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Flows_FlowId",
                table: "Actions",
                column: "FlowId",
                principalTable: "Flows",
                principalColumn: "FlowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flows_Events_EventId",
                table: "Flows",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowsRun_Flows_FlowId",
                table: "FlowsRun",
                column: "FlowId",
                principalTable: "Flows",
                principalColumn: "FlowId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Triggers_Flows_FlowId",
                table: "Triggers",
                column: "FlowId",
                principalTable: "Flows",
                principalColumn: "FlowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Flows_FlowId",
                table: "Actions");

            migrationBuilder.DropForeignKey(
                name: "FK_Flows_Events_EventId",
                table: "Flows");

            migrationBuilder.DropForeignKey(
                name: "FK_FlowsRun_Flows_FlowId",
                table: "FlowsRun");

            migrationBuilder.DropForeignKey(
                name: "FK_Triggers_Flows_FlowId",
                table: "Triggers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Flows",
                table: "Flows");

            migrationBuilder.DropIndex(
                name: "IX_Flows_EventId",
                table: "Flows");

            migrationBuilder.DropColumn(
                name: "FlowId",
                table: "Flows");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Flows");

            migrationBuilder.DropColumn(
                name: "multipleRuns",
                table: "Flows");

            migrationBuilder.DropColumn(
                name: "stillPending",
                table: "Flows");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Flows",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Flows",
                table: "Flows",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Flows_FlowId",
                table: "Actions",
                column: "FlowId",
                principalTable: "Flows",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowsRun_Flows_FlowId",
                table: "FlowsRun",
                column: "FlowId",
                principalTable: "Flows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Triggers_Flows_FlowId",
                table: "Triggers",
                column: "FlowId",
                principalTable: "Flows",
                principalColumn: "Id");
        }
    }
}
