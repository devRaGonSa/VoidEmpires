using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoidEmpires.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanetOwnershipModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "planet_ownerships",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    planet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    civilization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    claimed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planet_ownerships", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_planet_ownerships_civilization_id",
                table: "planet_ownerships",
                column: "civilization_id");

            migrationBuilder.CreateIndex(
                name: "ux_planet_ownerships_planet_id",
                table: "planet_ownerships",
                column: "planet_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "planet_ownerships");
        }
    }
}
