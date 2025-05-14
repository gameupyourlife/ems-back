using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ems_back.Repo.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTypeOfArrayOfMail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Default-Wert entfernen
            migrationBuilder.Sql(
                "ALTER TABLE \"Mail\" ALTER COLUMN \"Recipients\" DROP DEFAULT;");

            // 2. Typ ändern (mit USING)
            migrationBuilder.Sql(
                "ALTER TABLE \"Mail\" ALTER COLUMN \"Recipients\" TYPE uuid[] USING \"Recipients\"::uuid[];");

            // 3. (Optional) Neuer Default-Wert setzen – falls du einen brauchst
            // migrationBuilder.Sql("ALTER TABLE \"Mail\" ALTER COLUMN \"Recipients\" SET DEFAULT ARRAY[]::uuid[];");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE \"Mail\" ALTER COLUMN \"Recipients\" DROP DEFAULT;");

            migrationBuilder.Sql(
                "ALTER TABLE \"Mail\" ALTER COLUMN \"Recipients\" TYPE text[] USING \"Recipients\"::text[];");

            // Optional: alten Default setzen
            // migrationBuilder.Sql("ALTER TABLE \"Mail\" ALTER COLUMN \"Recipients\" SET DEFAULT ARRAY[]::text[];");
        }
    }
}
