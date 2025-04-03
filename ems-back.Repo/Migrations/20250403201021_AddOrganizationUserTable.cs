using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ems_back.Repo.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    OrganizationAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    OrganizationDescription = table.Column<string>(type: "text", nullable: true),
                    OrganizationProfilePicture = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OrganizationWebsite = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserFirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserLastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserEmail = table.Column<string>(type: "text", nullable: false),
                    UserRole = table.Column<int>(type: "integer", nullable: false),
                    UserProfilePicture = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsOrganizationAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationUsers_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUsers_IsOrganizationAdmin",
                table: "OrganizationUsers",
                column: "IsOrganizationAdmin");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUsers_OrganizationId_UserId",
                table: "OrganizationUsers",
                columns: new[] { "OrganizationId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUsers_UserEmail",
                table: "OrganizationUsers",
                column: "UserEmail");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUsers_UserId",
                table: "OrganizationUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUsers_UserRole",
                table: "OrganizationUsers",
                column: "UserRole");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationUsers");
        }
    }
}
