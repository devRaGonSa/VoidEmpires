using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoidEmpires.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExplorationKnowledgeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExplorationKnowledge",
                columns: table => new { Id = table.Column<Guid>(type: "uuid", nullable: false), CivilizationId = table.Column<Guid>(type: "uuid", nullable: false), SystemId = table.Column<Guid>(type: "uuid", nullable: false), PlanetId = table.Column<Guid>(type: "uuid", nullable: true), Source = table.Column<int>(type: "integer", nullable: false), SourceMissionId = table.Column<Guid>(type: "uuid", nullable: true), DiscoveredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false) },
                constraints: table => table.PrimaryKey("PK_ExplorationKnowledge", x => x.Id));

            migrationBuilder.CreateIndex(name: "IX_ExplorationKnowledge_CivilizationId", table: "ExplorationKnowledge", column: "CivilizationId");
            migrationBuilder.CreateIndex(name: "IX_ExplorationKnowledge_SystemId", table: "ExplorationKnowledge", column: "SystemId");
            migrationBuilder.CreateIndex(name: "IX_ExplorationKnowledge_PlanetId", table: "ExplorationKnowledge", column: "PlanetId");
            migrationBuilder.CreateIndex(name: "IX_ExplorationKnowledge_CivilizationId_SystemId", table: "ExplorationKnowledge", columns: new[] { "CivilizationId", "SystemId" }, unique: true, filter: "\"PlanetId\" IS NULL");
            migrationBuilder.CreateIndex(name: "IX_ExplorationKnowledge_CivilizationId_SystemId_PlanetId", table: "ExplorationKnowledge", columns: new[] { "CivilizationId", "SystemId", "PlanetId" }, unique: true, filter: "\"PlanetId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ExplorationKnowledge");
        }
    }
}
