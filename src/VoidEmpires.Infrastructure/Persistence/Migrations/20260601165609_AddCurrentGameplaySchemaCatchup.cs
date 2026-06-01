using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoidEmpires.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentGameplaySchemaCatchup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllianceMemberships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AllianceId = table.Column<Guid>(type: "uuid", nullable: false),
                    CivilizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    JoinedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllianceMemberships", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlliancePacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceAllianceId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetAllianceId = table.Column<Guid>(type: "uuid", nullable: false),
                    PactType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlliancePacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alliances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Tag = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alliances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetProductionOrder",
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
                    table.PrimaryKey("PK_AssetProductionOrder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiplomaticContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CivilizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContactedCivilizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DiscoveredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Source = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiplomaticContacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExplorationKnowledge",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CivilizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    SystemId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanetId = table.Column<Guid>(type: "uuid", nullable: true),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    SourceMissionId = table.Column<Guid>(type: "uuid", nullable: true),
                    DiscoveredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExplorationKnowledge", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "OrbitalAssetStock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetType = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrbitalAssetStock", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrbitalGroup",
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
                    table.PrimaryKey("PK_OrbitalGroup", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "PlanetaryAssetStock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetType = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanetaryAssetStock", x => x.Id);
                });

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
                name: "IX_AllianceMemberships_AllianceId",
                table: "AllianceMemberships",
                column: "AllianceId");

            migrationBuilder.CreateIndex(
                name: "IX_AllianceMemberships_AllianceId_CivilizationId",
                table: "AllianceMemberships",
                columns: new[] { "AllianceId", "CivilizationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AllianceMemberships_CivilizationId",
                table: "AllianceMemberships",
                column: "CivilizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AllianceMemberships_CivilizationId_JoinedAtUtc_Id",
                table: "AllianceMemberships",
                columns: new[] { "CivilizationId", "JoinedAtUtc", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_AlliancePacts_SourceAllianceId_Status_CreatedAtUtc_Id",
                table: "AlliancePacts",
                columns: new[] { "SourceAllianceId", "Status", "CreatedAtUtc", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_AlliancePacts_SourceAllianceId_TargetAllianceId_PactType",
                table: "AlliancePacts",
                columns: new[] { "SourceAllianceId", "TargetAllianceId", "PactType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlliancePacts_TargetAllianceId_Status_CreatedAtUtc_Id",
                table: "AlliancePacts",
                columns: new[] { "TargetAllianceId", "Status", "CreatedAtUtc", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Alliances_Name",
                table: "Alliances",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Alliances_Status_CreatedAtUtc_Id",
                table: "Alliances",
                columns: new[] { "Status", "CreatedAtUtc", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Alliances_Tag",
                table: "Alliances",
                column: "Tag",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetProductionOrder_PlanetId",
                table: "AssetProductionOrder",
                column: "PlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetProductionOrder_PlanetId_Sequence",
                table: "AssetProductionOrder",
                columns: new[] { "PlanetId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetProductionOrder_PlanetId_Status",
                table: "AssetProductionOrder",
                columns: new[] { "PlanetId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_DiplomaticContacts_CivilizationId",
                table: "DiplomaticContacts",
                column: "CivilizationId");

            migrationBuilder.CreateIndex(
                name: "IX_DiplomaticContacts_CivilizationId_ContactedCivilizationId",
                table: "DiplomaticContacts",
                columns: new[] { "CivilizationId", "ContactedCivilizationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiplomaticContacts_ContactedCivilizationId",
                table: "DiplomaticContacts",
                column: "ContactedCivilizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExplorationKnowledge_CivilizationId",
                table: "ExplorationKnowledge",
                column: "CivilizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExplorationKnowledge_CivilizationId_SystemId",
                table: "ExplorationKnowledge",
                columns: new[] { "CivilizationId", "SystemId" },
                unique: true,
                filter: "\"PlanetId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ExplorationKnowledge_CivilizationId_SystemId_PlanetId",
                table: "ExplorationKnowledge",
                columns: new[] { "CivilizationId", "SystemId", "PlanetId" },
                unique: true,
                filter: "\"PlanetId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ExplorationKnowledge_PlanetId",
                table: "ExplorationKnowledge",
                column: "PlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_ExplorationKnowledge_SystemId",
                table: "ExplorationKnowledge",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_ExplorationMissions_CivilizationId",
                table: "ExplorationMissions",
                column: "CivilizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExplorationMissions_CivilizationId_Status_DueAtUtc",
                table: "ExplorationMissions",
                columns: new[] { "CivilizationId", "Status", "DueAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ExplorationMissions_TargetPlanetId",
                table: "ExplorationMissions",
                column: "TargetPlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_ExplorationMissions_TargetSystemId",
                table: "ExplorationMissions",
                column: "TargetSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrbitalAssetStock_PlanetId_AssetType",
                table: "OrbitalAssetStock",
                columns: new[] { "PlanetId", "AssetType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrbitalGroup_CivilizationId",
                table: "OrbitalGroup",
                column: "CivilizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrbitalGroup_CurrentPlanetId",
                table: "OrbitalGroup",
                column: "CurrentPlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_OrbitalGroup_CurrentPlanetId_Status",
                table: "OrbitalGroup",
                columns: new[] { "CurrentPlanetId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_OrbitalGroup_OriginPlanetId",
                table: "OrbitalGroup",
                column: "OriginPlanetId");

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

            migrationBuilder.CreateIndex(
                name: "IX_PlanetaryAssetStock_PlanetId_AssetType",
                table: "PlanetaryAssetStock",
                columns: new[] { "PlanetId", "AssetType" },
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_PlanetPopulationProfiles_PlanetId",
                table: "PlanetPopulationProfiles",
                column: "PlanetId",
                unique: true);

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
                name: "AllianceMemberships");

            migrationBuilder.DropTable(
                name: "AlliancePacts");

            migrationBuilder.DropTable(
                name: "Alliances");

            migrationBuilder.DropTable(
                name: "AssetProductionOrder");

            migrationBuilder.DropTable(
                name: "DiplomaticContacts");

            migrationBuilder.DropTable(
                name: "ExplorationKnowledge");

            migrationBuilder.DropTable(
                name: "ExplorationMissions");

            migrationBuilder.DropTable(
                name: "OrbitalAssetStock");

            migrationBuilder.DropTable(
                name: "OrbitalGroup");

            migrationBuilder.DropTable(
                name: "OrbitalTransfers");

            migrationBuilder.DropTable(
                name: "PlanetaryAssetStock");

            migrationBuilder.DropTable(
                name: "PlanetConstructionOrders");

            migrationBuilder.DropTable(
                name: "PlanetPopulationProfiles");

            migrationBuilder.DropTable(
                name: "ResearchOrders");
        }
    }
}
