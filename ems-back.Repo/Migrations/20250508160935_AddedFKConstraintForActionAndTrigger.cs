using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ems_back.Repo.Migrations
{
    /// <inheritdoc />
    public partial class AddedFKConstraintForActionAndTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Flows_FlowTemplateId",
                table: "Flows");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_FlowTemplateId",
                table: "Flows",
                column: "FlowTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Flows_FlowTemplateId",
                table: "Flows");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_FlowTemplateId",
                table: "Flows",
                column: "FlowTemplateId",
                unique: true);
        }
    }
}
