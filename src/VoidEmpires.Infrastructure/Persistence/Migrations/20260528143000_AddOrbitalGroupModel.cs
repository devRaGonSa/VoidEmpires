using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoidEmpires.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrbitalGroupModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrbitalGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CivilizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginPlanetId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentPlanetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetType = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrbitalGroups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrbitalGroups_CivilizationId",
                table: "OrbitalGroups",
                column: "CivilizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrbitalGroups_OriginPlanetId",
                table: "OrbitalGroups",
                column: "OriginPlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_OrbitalGroups_CurrentPlanetId",
                table: "OrbitalGroups",
                column: "CurrentPlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_OrbitalGroups_CurrentPlanetId_Status",
                table: "OrbitalGroups",
                columns: new[] { "CurrentPlanetId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "OrbitalGroups");
        }
    }
}
