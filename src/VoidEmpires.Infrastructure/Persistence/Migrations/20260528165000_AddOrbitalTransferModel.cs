using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoidEmpires.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrbitalTransferModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrbitalTransfers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CivilizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrbitalGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginPlanetId = table.Column<Guid>(type: "uuid", nullable: false),
                    DestinationPlanetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AbstractDistanceUnits = table.Column<int>(type: "integer", nullable: false),
                    DepartureAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ArrivalAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrbitalTransfers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrbitalTransfers_CivilizationId",
                table: "OrbitalTransfers",
                column: "CivilizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrbitalTransfers_DestinationPlanetId",
                table: "OrbitalTransfers",
                column: "DestinationPlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_OrbitalTransfers_OrbitalGroupId",
                table: "OrbitalTransfers",
                column: "OrbitalGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_OrbitalTransfers_OriginPlanetId",
                table: "OrbitalTransfers",
                column: "OriginPlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_OrbitalTransfers_Status_ArrivalAtUtc",
                table: "OrbitalTransfers",
                columns: new[] { "Status", "ArrivalAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "OrbitalTransfers");
        }
    }
}
