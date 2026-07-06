/*
VoidEmpires SQL Server post-apply verification.

Run manually in SSMS after an operator applies the reviewed baseline script.
These checks are read-only and contain no connection strings or credentials.
They do not prove the repository applied the schema automatically.
*/

SELECT DB_NAME() AS CurrentDatabase;

SELECT
    s.[name] AS SchemaName,
    t.[name] AS TableName
FROM sys.tables AS t
INNER JOIN sys.schemas AS s ON s.[schema_id] = t.[schema_id]
WHERE t.is_ms_shipped = 0
ORDER BY s.[name], t.[name];

WITH ExpectedTables AS (
    SELECT TableName
    FROM (VALUES
        (N'__EFMigrationsHistory'),
        (N'Alliances'),
        (N'AllianceMemberships'),
        (N'AlliancePacts'),
        (N'AspNetRoles'),
        (N'AspNetUsers'),
        (N'AspNetRoleClaims'),
        (N'AspNetUserClaims'),
        (N'AspNetUserLogins'),
        (N'AspNetUserRoles'),
        (N'AspNetUserTokens'),
        (N'AssetProductionOrder'),
        (N'civilizations'),
        (N'DiplomaticContacts'),
        (N'exploration_knowledge'),
        (N'ExplorationMissions'),
        (N'galaxies'),
        (N'OrbitalAssetStock'),
        (N'OrbitalGroup'),
        (N'OrbitalTransfers'),
        (N'planet_ownerships'),
        (N'planet_production_profiles'),
        (N'planet_resource_stockpiles'),
        (N'PlanetaryAssetStock'),
        (N'PlanetBuildingCapacities'),
        (N'PlanetBuildings'),
        (N'PlanetConstructionOrders'),
        (N'PlanetPopulationProfiles'),
        (N'planets'),
        (N'player_profiles'),
        (N'ResearchOrders'),
        (N'ResearchProjects'),
        (N'solar_systems'),
        (N'stars')
    ) AS v(TableName)
)
SELECT
    e.TableName,
    CASE WHEN t.[object_id] IS NULL THEN N'MISSING' ELSE N'PRESENT' END AS Status
FROM ExpectedTables AS e
LEFT JOIN sys.tables AS t ON t.[name] = e.TableName
ORDER BY e.TableName;

SELECT
    [MigrationId],
    [ProductVersion]
FROM [dbo].[__EFMigrationsHistory]
WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
ORDER BY [MigrationId];

SELECT N'AspNetUsers' AS TableName, COUNT_BIG(*) AS RowCount FROM [dbo].[AspNetUsers]
UNION ALL SELECT N'player_profiles', COUNT_BIG(*) FROM [dbo].[player_profiles]
UNION ALL SELECT N'civilizations', COUNT_BIG(*) FROM [dbo].[civilizations]
UNION ALL SELECT N'galaxies', COUNT_BIG(*) FROM [dbo].[galaxies]
UNION ALL SELECT N'solar_systems', COUNT_BIG(*) FROM [dbo].[solar_systems]
UNION ALL SELECT N'planets', COUNT_BIG(*) FROM [dbo].[planets]
UNION ALL SELECT N'stars', COUNT_BIG(*) FROM [dbo].[stars]
UNION ALL SELECT N'planet_ownerships', COUNT_BIG(*) FROM [dbo].[planet_ownerships]
UNION ALL SELECT N'planet_resource_stockpiles', COUNT_BIG(*) FROM [dbo].[planet_resource_stockpiles]
UNION ALL SELECT N'PlanetConstructionOrders', COUNT_BIG(*) FROM [dbo].[PlanetConstructionOrders]
UNION ALL SELECT N'ResearchOrders', COUNT_BIG(*) FROM [dbo].[ResearchOrders]
UNION ALL SELECT N'AssetProductionOrder', COUNT_BIG(*) FROM [dbo].[AssetProductionOrder]
UNION ALL SELECT N'OrbitalGroup', COUNT_BIG(*) FROM [dbo].[OrbitalGroup]
UNION ALL SELECT N'OrbitalTransfers', COUNT_BIG(*) FROM [dbo].[OrbitalTransfers]
UNION ALL SELECT N'Alliances', COUNT_BIG(*) FROM [dbo].[Alliances]
UNION ALL SELECT N'DiplomaticContacts', COUNT_BIG(*) FROM [dbo].[DiplomaticContacts]
UNION ALL SELECT N'ExplorationMissions', COUNT_BIG(*) FROM [dbo].[ExplorationMissions];

WITH ExpectedIndexes AS (
    SELECT TableName, IndexName
    FROM (VALUES
        (N'PlanetConstructionOrders', N'IX_PlanetConstructionOrders_Status_EndsAtUtc_Sequence'),
        (N'ResearchOrders', N'IX_ResearchOrders_Status_EndsAtUtc_Sequence'),
        (N'AssetProductionOrder', N'IX_AssetProductionOrder_Target_Status_EndsAtUtc_Sequence'),
        (N'planet_ownerships', N'ix_planet_ownerships_civilization_status'),
        (N'planet_ownerships', N'ux_planet_ownerships_planet_id'),
        (N'player_profiles', N'ux_player_profiles_normalized_display_name'),
        (N'civilizations', N'ix_civilizations_normalized_name'),
        (N'civilizations', N'ux_civilizations_profile_normalized_name'),
        (N'OrbitalAssetStock', N'IX_OrbitalAssetStock_PlanetId_AssetType'),
        (N'PlanetaryAssetStock', N'IX_PlanetaryAssetStock_PlanetId_AssetType'),
        (N'ResearchProjects', N'IX_ResearchProjects_CivilizationId_ResearchType')
    ) AS v(TableName, IndexName)
)
SELECT
    e.TableName,
    e.IndexName,
    CASE WHEN i.[index_id] IS NULL THEN N'MISSING' ELSE N'PRESENT' END AS Status
FROM ExpectedIndexes AS e
LEFT JOIN sys.tables AS t ON t.[name] = e.TableName
LEFT JOIN sys.indexes AS i ON i.[object_id] = t.[object_id] AND i.[name] = e.IndexName
ORDER BY e.TableName, e.IndexName;

SELECT
    s.[name] AS SchemaName,
    t.[name] AS CatalogLikeTableName
FROM sys.tables AS t
INNER JOIN sys.schemas AS s ON s.[schema_id] = t.[schema_id]
WHERE t.is_ms_shipped = 0
  AND (
      t.[name] LIKE N'%Catalog%'
      OR t.[name] LIKE N'%Seed%'
  )
ORDER BY s.[name], t.[name];
