using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoidEmpires.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanetEconomyModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "planet_production_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    planet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    credits_per_hour = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    metal_per_hour = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    crystal_per_hour = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    gas_per_hour = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planet_production_profiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "planet_resource_stockpiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    planet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    credits = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    metal = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    crystal = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    gas = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planet_resource_stockpiles", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ux_planet_production_profiles_planet_id",
                table: "planet_production_profiles",
                column: "planet_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_planet_resource_stockpiles_planet_id",
                table: "planet_resource_stockpiles",
                column: "planet_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "planet_production_profiles");

            migrationBuilder.DropTable(
                name: "planet_resource_stockpiles");
        }
    }
}
