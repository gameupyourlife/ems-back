using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ems_back.Repo.Migrations
{
    /// <inheritdoc />
    public partial class AddedAttendeeInfosToEventModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttendeeCount",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttendeeCount",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Events");
        }
    }
}
