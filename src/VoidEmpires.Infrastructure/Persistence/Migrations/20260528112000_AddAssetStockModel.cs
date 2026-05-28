using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoidEmpires.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetStockModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlanetaryAssetStocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetType = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanetaryAssetStocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrbitalAssetStocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetType = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrbitalAssetStocks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanetaryAssetStocks_PlanetId_AssetType",
                table: "PlanetaryAssetStocks",
                columns: new[] { "PlanetId", "AssetType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrbitalAssetStocks_PlanetId_AssetType",
                table: "OrbitalAssetStocks",
                columns: new[] { "PlanetId", "AssetType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PlanetaryAssetStocks");
            migrationBuilder.DropTable(name: "OrbitalAssetStocks");
        }
    }
}
