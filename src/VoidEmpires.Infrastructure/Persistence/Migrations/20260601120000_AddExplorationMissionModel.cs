using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoidEmpires.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExplorationMissionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExplorationMissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CivilizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetSystemId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetPlanetId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DueAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExplorationMissions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExplorationMissions_CivilizationId",
                table: "ExplorationMissions",
                column: "CivilizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExplorationMissions_TargetSystemId",
                table: "ExplorationMissions",
                column: "TargetSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_ExplorationMissions_TargetPlanetId",
                table: "ExplorationMissions",
                column: "TargetPlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_ExplorationMissions_CivilizationId_Status_DueAtUtc",
                table: "ExplorationMissions",
                columns: new[] { "CivilizationId", "Status", "DueAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ExplorationMissions");
        }
    }
}
