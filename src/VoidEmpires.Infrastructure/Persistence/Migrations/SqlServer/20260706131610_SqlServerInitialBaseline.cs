using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoidEmpires.Infrastructure.Persistence.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class SqlServerInitialBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllianceMemberships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllianceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CivilizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    JoinedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllianceMemberships", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlliancePacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceAllianceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetAllianceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PactType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlliancePacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alliances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alliances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetProductionOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Target = table.Column<int>(type: "int", nullable: false),
                    PlanetaryAssetType = table.Column<int>(type: "int", nullable: true),
                    SpaceAssetType = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    StartsAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndsAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetProductionOrder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiplomaticContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CivilizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactedCivilizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DiscoveredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiplomaticContacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "exploration_knowledge",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    civilization_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    system_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    planet_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    source = table.Column<int>(type: "int", nullable: false),
                    source_mission_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    discovered_at_utc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exploration_knowledge", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ExplorationMissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CivilizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetSystemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetPlanetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExplorationMissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "galaxies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_galaxies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "OrbitalAssetStock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetType = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrbitalAssetStock", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrbitalGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CivilizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginPlanetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentPlanetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetType = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrbitalGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrbitalTransfers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CivilizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrbitalGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginPlanetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DestinationPlanetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AbstractDistanceUnits = table.Column<int>(type: "int", nullable: false),
                    DepartureAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrivalAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrbitalTransfers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "planet_ownerships",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    planet_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    civilization_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    claimed_at_utc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planet_ownerships", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "planet_production_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    planet_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    credits_per_hour = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    metal_per_hour = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    crystal_per_hour = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    gas_per_hour = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planet_production_profiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "planet_resource_stockpiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    planet_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    credits = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    metal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    crystal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    gas = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planet_resource_stockpiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PlanetaryAssetStock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetType = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanetaryAssetStock", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanetBuildingCapacities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseCapacity = table.Column<int>(type: "int", nullable: false),
                    BonusCapacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanetBuildingCapacities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanetBuildings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuildingType = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Footprint = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanetBuildings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanetConstructionOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    BuildingType = table.Column<int>(type: "int", nullable: false),
                    TargetLevel = table.Column<int>(type: "int", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    StartsAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndsAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanetConstructionOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanetPopulationProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalPopulation = table.Column<long>(type: "bigint", nullable: false),
                    BaseRecruitablePopulation = table.Column<long>(type: "bigint", nullable: false),
                    BaseCrewCapacity = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanetPopulationProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "player_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    display_name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    normalized_display_name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_profiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ResearchOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CivilizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourcePlanetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResearchType = table.Column<int>(type: "int", nullable: false),
                    TargetLevel = table.Column<int>(type: "int", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    StartsAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndsAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResearchOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResearchProjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CivilizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResearchType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResearchProjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "solar_systems",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    galaxy_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    coordinates_x = table.Column<int>(type: "int", nullable: false),
                    coordinates_y = table.Column<int>(type: "int", nullable: false),
                    coordinates_z = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_solar_systems", x => x.id);
                    table.ForeignKey(
                        name: "FK_solar_systems_galaxies_galaxy_id",
                        column: x => x.galaxy_id,
                        principalTable: "galaxies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "civilizations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    player_profile_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    normalized_name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    archetype = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    home_planet_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_civilizations", x => x.id);
                    table.ForeignKey(
                        name: "FK_civilizations_player_profiles_player_profile_id",
                        column: x => x.player_profile_id,
                        principalTable: "player_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "planets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    solar_system_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    orbital_slot = table.Column<int>(type: "int", nullable: false),
                    planet_type = table.Column<int>(type: "int", nullable: false),
                    size = table.Column<int>(type: "int", nullable: false),
                    colonization_status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planets", x => x.id);
                    table.ForeignKey(
                        name: "FK_planets_solar_systems_solar_system_id",
                        column: x => x.solar_system_id,
                        principalTable: "solar_systems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stars",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    solar_system_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    star_type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stars", x => x.id);
                    table.ForeignKey(
                        name: "FK_stars_solar_systems_solar_system_id",
                        column: x => x.solar_system_id,
                        principalTable: "solar_systems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

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
                name: "IX_AssetProductionOrder_Target_Status_EndsAtUtc_Sequence",
                table: "AssetProductionOrder",
                columns: new[] { "Target", "Status", "EndsAtUtc", "Sequence" });

            migrationBuilder.CreateIndex(
                name: "ix_civilizations_normalized_name",
                table: "civilizations",
                column: "normalized_name");

            migrationBuilder.CreateIndex(
                name: "ux_civilizations_profile_normalized_name",
                table: "civilizations",
                columns: new[] { "player_profile_id", "normalized_name" },
                unique: true);

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
                name: "IX_exploration_knowledge_civilization_id",
                table: "exploration_knowledge",
                column: "civilization_id");

            migrationBuilder.CreateIndex(
                name: "IX_exploration_knowledge_civilization_id_system_id",
                table: "exploration_knowledge",
                columns: new[] { "civilization_id", "system_id" },
                unique: true,
                filter: "planet_id IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_exploration_knowledge_civilization_id_system_id_planet_id",
                table: "exploration_knowledge",
                columns: new[] { "civilization_id", "system_id", "planet_id" },
                unique: true,
                filter: "planet_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_exploration_knowledge_planet_id",
                table: "exploration_knowledge",
                column: "planet_id");

            migrationBuilder.CreateIndex(
                name: "IX_exploration_knowledge_system_id",
                table: "exploration_knowledge",
                column: "system_id");

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
                name: "ix_planet_ownerships_civilization_status",
                table: "planet_ownerships",
                columns: new[] { "civilization_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ux_planet_ownerships_planet_id",
                table: "planet_ownerships",
                column: "planet_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_planet_production_profiles_planet_id",
                table: "planet_production_profiles",
                column: "planet_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_planet_resource_stockpiles_planet_id",
                table: "planet_resource_stockpiles",
                column: "planet_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanetaryAssetStock_PlanetId_AssetType",
                table: "PlanetaryAssetStock",
                columns: new[] { "PlanetId", "AssetType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanetBuildingCapacities_PlanetId",
                table: "PlanetBuildingCapacities",
                column: "PlanetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanetBuildings_PlanetId",
                table: "PlanetBuildings",
                column: "PlanetId");

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
                name: "IX_PlanetConstructionOrders_Status_EndsAtUtc_Sequence",
                table: "PlanetConstructionOrders",
                columns: new[] { "Status", "EndsAtUtc", "Sequence" });

            migrationBuilder.CreateIndex(
                name: "IX_PlanetPopulationProfiles_PlanetId",
                table: "PlanetPopulationProfiles",
                column: "PlanetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_planets_solar_system_orbital_slot",
                table: "planets",
                columns: new[] { "solar_system_id", "orbital_slot" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_player_profiles_normalized_display_name",
                table: "player_profiles",
                column: "normalized_display_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_player_profiles_user_id",
                table: "player_profiles",
                column: "user_id",
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

            migrationBuilder.CreateIndex(
                name: "IX_ResearchOrders_Status_EndsAtUtc_Sequence",
                table: "ResearchOrders",
                columns: new[] { "Status", "EndsAtUtc", "Sequence" });

            migrationBuilder.CreateIndex(
                name: "IX_ResearchProjects_CivilizationId_ResearchType",
                table: "ResearchProjects",
                columns: new[] { "CivilizationId", "ResearchType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_solar_systems_galaxy_coordinates",
                table: "solar_systems",
                columns: new[] { "galaxy_id", "coordinates_x", "coordinates_y", "coordinates_z" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_stars_solar_system_id",
                table: "stars",
                column: "solar_system_id",
                unique: true);
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
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AssetProductionOrder");

            migrationBuilder.DropTable(
                name: "civilizations");

            migrationBuilder.DropTable(
                name: "DiplomaticContacts");

            migrationBuilder.DropTable(
                name: "exploration_knowledge");

            migrationBuilder.DropTable(
                name: "ExplorationMissions");

            migrationBuilder.DropTable(
                name: "OrbitalAssetStock");

            migrationBuilder.DropTable(
                name: "OrbitalGroup");

            migrationBuilder.DropTable(
                name: "OrbitalTransfers");

            migrationBuilder.DropTable(
                name: "planet_ownerships");

            migrationBuilder.DropTable(
                name: "planet_production_profiles");

            migrationBuilder.DropTable(
                name: "planet_resource_stockpiles");

            migrationBuilder.DropTable(
                name: "PlanetaryAssetStock");

            migrationBuilder.DropTable(
                name: "PlanetBuildingCapacities");

            migrationBuilder.DropTable(
                name: "PlanetBuildings");

            migrationBuilder.DropTable(
                name: "PlanetConstructionOrders");

            migrationBuilder.DropTable(
                name: "PlanetPopulationProfiles");

            migrationBuilder.DropTable(
                name: "planets");

            migrationBuilder.DropTable(
                name: "ResearchOrders");

            migrationBuilder.DropTable(
                name: "ResearchProjects");

            migrationBuilder.DropTable(
                name: "stars");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "player_profiles");

            migrationBuilder.DropTable(
                name: "solar_systems");

            migrationBuilder.DropTable(
                name: "galaxies");
        }
    }
}
