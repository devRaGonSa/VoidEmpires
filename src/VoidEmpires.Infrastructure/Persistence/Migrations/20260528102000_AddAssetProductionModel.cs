using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoidEmpires.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetProductionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetProductionOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Target = table.Column<int>(type: "integer", nullable: false),
                    PlanetaryAssetType = table.Column<int>(type: "integer", nullable: true),
                    SpaceAssetType = table.Column<int>(type: "integer", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Sequence = table.Column<int>(type: "integer", nullable: false),
                    StartsAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndsAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetProductionOrders", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetProductionOrders_PlanetId",
                table: "AssetProductionOrders",
                column: "PlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetProductionOrders_PlanetId_Sequence",
                table: "AssetProductionOrders",
                columns: new[] { "PlanetId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetProductionOrders_PlanetId_Status",
                table: "AssetProductionOrders",
                columns: new[] { "PlanetId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetProductionOrders");
        }
    }
}
