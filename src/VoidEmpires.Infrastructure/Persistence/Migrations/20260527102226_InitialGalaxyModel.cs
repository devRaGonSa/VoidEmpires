using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoidEmpires.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialGalaxyModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "galaxies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_galaxies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "solar_systems",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    galaxy_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    coordinates_x = table.Column<int>(type: "integer", nullable: false),
                    coordinates_y = table.Column<int>(type: "integer", nullable: false),
                    coordinates_z = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_solar_systems", x => x.id);
                    table.ForeignKey(
                        name: "FK_solar_systems_galaxies_galaxy_id",
                        column: x => x.galaxy_id,
                        principalTable: "galaxies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "planets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    solar_system_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    orbital_slot = table.Column<int>(type: "integer", nullable: false),
                    planet_type = table.Column<int>(type: "integer", nullable: false),
                    size = table.Column<int>(type: "integer", nullable: false),
                    colonization_status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planets", x => x.id);
                    table.ForeignKey(
                        name: "FK_planets_solar_systems_solar_system_id",
                        column: x => x.solar_system_id,
                        principalTable: "solar_systems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stars",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    solar_system_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    star_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stars", x => x.id);
                    table.ForeignKey(
                        name: "FK_stars_solar_systems_solar_system_id",
                        column: x => x.solar_system_id,
                        principalTable: "solar_systems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ux_planets_solar_system_orbital_slot",
                table: "planets",
                columns: new[] { "solar_system_id", "orbital_slot" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_solar_systems_galaxy_coordinates",
                table: "solar_systems",
                columns: new[] { "galaxy_id", "coordinates_x", "coordinates_y", "coordinates_z" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_stars_solar_system_id",
                table: "stars",
                column: "solar_system_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "planets");

            migrationBuilder.DropTable(
                name: "stars");

            migrationBuilder.DropTable(
                name: "solar_systems");

            migrationBuilder.DropTable(
                name: "galaxies");
        }
    }
}
