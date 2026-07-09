/*
Generated for manual review only.
Do not execute without operator approval.

Purpose:
  Repair legacy duplicate PlanetBuildings rows created by non-idempotent Block 49
  gameplay refresh processing.

Notes:
  - This script keeps one row per PlanetId + BuildingType.
  - The kept row is updated to preserve the highest Level and max Footprint.
  - Duplicate rows are deleted after the keeper row is updated.
  - ResearchProjects already has a unique index on CivilizationId + ResearchType;
    runtime code now updates existing projects instead of inserting duplicates.
  - This script does not insert or change secrets.
  - Apply manually in SSMS against the intended VoidEmpires_Dev database.
*/

SET XACT_ABORT ON;

IF OBJECT_ID(N'[dbo].[PlanetBuildings]', N'U') IS NULL
BEGIN
    THROW 51050, 'Expected table [dbo].[PlanetBuildings] was not found. Verify the SQL Server schema before running this repair script.', 1;
END;
GO

SELECT
    N'Before' AS diagnostic_phase,
    [PlanetId],
    [BuildingType],
    COUNT(*) AS duplicate_count,
    MIN([Level]) AS min_level,
    MAX([Level]) AS max_level,
    MIN([Footprint]) AS min_footprint,
    MAX([Footprint]) AS max_footprint
FROM [dbo].[PlanetBuildings]
GROUP BY [PlanetId], [BuildingType]
HAVING COUNT(*) > 1
ORDER BY [PlanetId], [BuildingType];
GO

BEGIN TRY
    BEGIN TRANSACTION;

    ;WITH DuplicateGroups AS
    (
        SELECT
            [PlanetId],
            [BuildingType],
            MAX([Level]) AS [PreservedLevel],
            MAX([Footprint]) AS [PreservedFootprint]
        FROM [dbo].[PlanetBuildings] WITH (UPDLOCK, HOLDLOCK)
        GROUP BY [PlanetId], [BuildingType]
        HAVING COUNT(*) > 1
    ),
    RankedBuildings AS
    (
        SELECT
            b.[Id],
            b.[PlanetId],
            b.[BuildingType],
            g.[PreservedLevel],
            g.[PreservedFootprint],
            ROW_NUMBER() OVER (
                PARTITION BY b.[PlanetId], b.[BuildingType]
                ORDER BY b.[Level] DESC, b.[Footprint] DESC, b.[Id] ASC
            ) AS [RowNumber]
        FROM [dbo].[PlanetBuildings] AS b
        INNER JOIN DuplicateGroups AS g
            ON g.[PlanetId] = b.[PlanetId]
            AND g.[BuildingType] = b.[BuildingType]
    )
    UPDATE keepers
    SET
        [Level] = ranked.[PreservedLevel],
        [Footprint] = ranked.[PreservedFootprint]
    FROM [dbo].[PlanetBuildings] AS keepers
    INNER JOIN RankedBuildings AS ranked
        ON ranked.[Id] = keepers.[Id]
    WHERE ranked.[RowNumber] = 1;

    ;WITH RankedBuildings AS
    (
        SELECT
            [Id],
            ROW_NUMBER() OVER (
                PARTITION BY [PlanetId], [BuildingType]
                ORDER BY [Level] DESC, [Footprint] DESC, [Id] ASC
            ) AS [RowNumber]
        FROM [dbo].[PlanetBuildings] WITH (UPDLOCK, HOLDLOCK)
    )
    DELETE duplicates
    FROM [dbo].[PlanetBuildings] AS duplicates
    INNER JOIN RankedBuildings AS ranked
        ON ranked.[Id] = duplicates.[Id]
    WHERE ranked.[RowNumber] > 1;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
    BEGIN
        ROLLBACK TRANSACTION;
    END;

    THROW;
END CATCH;
GO

SELECT
    N'After' AS diagnostic_phase,
    [PlanetId],
    [BuildingType],
    COUNT(*) AS duplicate_count,
    MIN([Level]) AS min_level,
    MAX([Level]) AS max_level,
    MIN([Footprint]) AS min_footprint,
    MAX([Footprint]) AS max_footprint
FROM [dbo].[PlanetBuildings]
GROUP BY [PlanetId], [BuildingType]
HAVING COUNT(*) > 1
ORDER BY [PlanetId], [BuildingType];
GO

SELECT
    N'Post-repair sample' AS diagnostic_phase,
    [PlanetId],
    [BuildingType],
    [Level],
    [Footprint]
FROM [dbo].[PlanetBuildings]
ORDER BY [PlanetId], [BuildingType];
