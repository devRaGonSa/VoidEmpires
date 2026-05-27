using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoidEmpires.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanetBuildingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlanetBuildingCapacities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanetId = table.Column<Guid>(type: "uuid", nullable: false),
                    BaseCapacity = table.Column<int>(type: "integer", nullable: false),
                    BonusCapacity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanetBuildingCapacities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanetBuildings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanetId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuildingType = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Footprint = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanetBuildings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanetBuildingCapacities_PlanetId",
                table: "PlanetBuildingCapacities",
                column: "PlanetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanetBuildings_PlanetId",
                table: "PlanetBuildings",
                column: "PlanetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanetBuildingCapacities");

            migrationBuilder.DropTable(
                name: "PlanetBuildings");
        }
    }
}
