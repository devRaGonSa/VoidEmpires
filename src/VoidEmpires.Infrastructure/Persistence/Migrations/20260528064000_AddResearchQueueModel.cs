using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoidEmpires.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddResearchQueueModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResearchOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CivilizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourcePlanetId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResearchType = table.Column<int>(type: "integer", nullable: false),
                    TargetLevel = table.Column<int>(type: "integer", nullable: false),
                    Sequence = table.Column<int>(type: "integer", nullable: false),
                    StartsAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndsAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResearchOrders", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResearchOrders_CivilizationId",
                table: "ResearchOrders",
                column: "CivilizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ResearchOrders_CivilizationId_Sequence",
                table: "ResearchOrders",
                columns: new[] { "CivilizationId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResearchOrders_CivilizationId_Status",
                table: "ResearchOrders",
                columns: new[] { "CivilizationId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResearchOrders");
        }
    }
}
