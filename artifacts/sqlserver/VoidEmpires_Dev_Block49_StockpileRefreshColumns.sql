/*
Generated for manual review only.
Do not execute without operator approval.

Purpose:
  Align an existing VoidEmpires_Dev SQL Server database with the Block 49 EF model
  by adding the missing planet_resource_stockpiles columns used by gameplay refresh:
    - capacity decimal(18,4) NOT NULL
    - last_accrued_at_utc datetime2 NOT NULL

Notes:
  - This script is idempotent for the target columns.
  - It does not create the baseline schema.
  - It does not insert or change secrets.
  - Apply manually in SSMS against the intended VoidEmpires_Dev database.
*/

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @stockpileObjectId int = OBJECT_ID(N'[dbo].[planet_resource_stockpiles]', N'U');

    IF @stockpileObjectId IS NULL
    BEGIN
        THROW 51049, 'Expected table [dbo].[planet_resource_stockpiles] was not found. Apply the SQL Server baseline before this Block 49 hotfix script.', 1;
    END;

    IF COL_LENGTH(N'dbo.planet_resource_stockpiles', N'capacity') IS NULL
    BEGIN
        ALTER TABLE [dbo].[planet_resource_stockpiles]
            ADD [capacity] decimal(18,4) NOT NULL
                CONSTRAINT [DF_planet_resource_stockpiles_capacity_block49]
                DEFAULT (CONVERT(decimal(18,4), 1000.0000)) WITH VALUES;
    END
    ELSE
    BEGIN
        UPDATE [dbo].[planet_resource_stockpiles]
        SET [capacity] = CONVERT(decimal(18,4), 1000.0000)
        WHERE [capacity] IS NULL;

        ALTER TABLE [dbo].[planet_resource_stockpiles]
            ALTER COLUMN [capacity] decimal(18,4) NOT NULL;

        IF NOT EXISTS (
            SELECT 1
            FROM sys.default_constraints AS dc
            INNER JOIN sys.columns AS c
                ON c.object_id = dc.parent_object_id
                AND c.default_object_id = dc.object_id
            WHERE dc.parent_object_id = @stockpileObjectId
                AND c.name = N'capacity'
        )
        BEGIN
            ALTER TABLE [dbo].[planet_resource_stockpiles]
                ADD CONSTRAINT [DF_planet_resource_stockpiles_capacity_block49]
                DEFAULT (CONVERT(decimal(18,4), 1000.0000)) FOR [capacity];
        END;
    END;

    IF COL_LENGTH(N'dbo.planet_resource_stockpiles', N'last_accrued_at_utc') IS NULL
    BEGIN
        ALTER TABLE [dbo].[planet_resource_stockpiles]
            ADD [last_accrued_at_utc] datetime2 NOT NULL
                CONSTRAINT [DF_planet_resource_stockpiles_last_accrued_at_utc_block49]
                DEFAULT (CURRENT_TIMESTAMP) WITH VALUES;
    END
    ELSE
    BEGIN
        UPDATE [dbo].[planet_resource_stockpiles]
        SET [last_accrued_at_utc] = SYSUTCDATETIME()
        WHERE [last_accrued_at_utc] IS NULL;

        ALTER TABLE [dbo].[planet_resource_stockpiles]
            ALTER COLUMN [last_accrued_at_utc] datetime2 NOT NULL;

        IF NOT EXISTS (
            SELECT 1
            FROM sys.default_constraints AS dc
            INNER JOIN sys.columns AS c
                ON c.object_id = dc.parent_object_id
                AND c.default_object_id = dc.object_id
            WHERE dc.parent_object_id = @stockpileObjectId
                AND c.name = N'last_accrued_at_utc'
        )
        BEGIN
            ALTER TABLE [dbo].[planet_resource_stockpiles]
                ADD CONSTRAINT [DF_planet_resource_stockpiles_last_accrued_at_utc_block49]
                DEFAULT (CURRENT_TIMESTAMP) FOR [last_accrued_at_utc];
        END;
    END;

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
    c.name AS column_name,
    t.name AS sql_type,
    c.precision,
    c.scale,
    c.is_nullable,
    dc.definition AS default_definition
FROM sys.columns AS c
INNER JOIN sys.types AS t
    ON t.user_type_id = c.user_type_id
LEFT JOIN sys.default_constraints AS dc
    ON dc.parent_object_id = c.object_id
    AND dc.parent_column_id = c.column_id
WHERE c.object_id = OBJECT_ID(N'[dbo].[planet_resource_stockpiles]', N'U')
    AND c.name IN (N'capacity', N'last_accrued_at_utc')
ORDER BY c.name;
