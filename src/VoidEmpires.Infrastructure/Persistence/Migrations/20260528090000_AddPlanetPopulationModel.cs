using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoidEmpires.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanetPopulationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlanetPopulationProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanetId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalPopulation = table.Column<long>(type: "bigint", nullable: false),
                    BaseRecruitablePopulation = table.Column<long>(type: "bigint", nullable: false),
                    BaseCrewCapacity = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanetPopulationProfiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanetPopulationProfiles_PlanetId",
                table: "PlanetPopulationProfiles",
                column: "PlanetId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanetPopulationProfiles");
        }
    }
}
