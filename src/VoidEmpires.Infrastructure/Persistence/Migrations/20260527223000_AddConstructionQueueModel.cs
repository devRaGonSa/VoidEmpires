using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoidEmpires.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddConstructionQueueModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlanetConstructionOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    BuildingType = table.Column<int>(type: "integer", nullable: false),
                    TargetLevel = table.Column<int>(type: "integer", nullable: false),
                    Sequence = table.Column<int>(type: "integer", nullable: false),
                    StartsAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndsAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanetConstructionOrders", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanetConstructionOrders_PlanetId",
                table: "PlanetConstructionOrders",
                column: "PlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanetConstructionOrders_PlanetId_Sequence",
                table: "PlanetConstructionOrders",
                columns: new[] { "PlanetId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanetConstructionOrders_PlanetId_Status",
                table: "PlanetConstructionOrders",
                columns: new[] { "PlanetId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanetConstructionOrders");
        }
    }
}
