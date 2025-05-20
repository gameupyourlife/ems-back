using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ems_back.Repo.Migrations
{
    /// <inheritdoc />
    public partial class EnableCascadeDeleteForEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Flows_FlowId",
                table: "Actions");

            migrationBuilder.DropForeignKey(
                name: "FK_Triggers_Flows_FlowId",
                table: "Triggers");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Flows_FlowId",
                table: "Actions",
                column: "FlowId",
                principalTable: "Flows",
                principalColumn: "FlowId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Triggers_Flows_FlowId",
                table: "Triggers",
                column: "FlowId",
                principalTable: "Flows",
                principalColumn: "FlowId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Flows_FlowId",
                table: "Actions");

            migrationBuilder.DropForeignKey(
                name: "FK_Triggers_Flows_FlowId",
                table: "Triggers");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Flows_FlowId",
                table: "Actions",
                column: "FlowId",
                principalTable: "Flows",
                principalColumn: "FlowId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Triggers_Flows_FlowId",
                table: "Triggers",
                column: "FlowId",
                principalTable: "Flows",
                principalColumn: "FlowId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
