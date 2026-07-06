-- VoidEmpires SQL Server initial baseline script.
-- Generated for manual review only by scripts/sqlserver-script-migration.ps1.
-- Do not execute without operator approval, backup review, and the manual SSMS apply runbook.
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [AllianceMemberships] (
        [Id] uniqueidentifier NOT NULL,
        [AllianceId] uniqueidentifier NOT NULL,
        [CivilizationId] uniqueidentifier NOT NULL,
        [Status] int NOT NULL,
        [Role] int NOT NULL,
        [JoinedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_AllianceMemberships] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [AlliancePacts] (
        [Id] uniqueidentifier NOT NULL,
        [SourceAllianceId] uniqueidentifier NOT NULL,
        [TargetAllianceId] uniqueidentifier NOT NULL,
        [PactType] int NOT NULL,
        [Status] int NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_AlliancePacts] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [Alliances] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [Tag] nvarchar(16) NOT NULL,
        [Status] int NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_Alliances] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [AssetProductionOrder] (
        [Id] uniqueidentifier NOT NULL,
        [PlanetId] uniqueidentifier NOT NULL,
        [Target] int NOT NULL,
        [PlanetaryAssetType] int NULL,
        [SpaceAssetType] int NULL,
        [Quantity] int NOT NULL,
        [Sequence] int NOT NULL,
        [StartsAtUtc] datetime2 NOT NULL,
        [EndsAtUtc] datetime2 NOT NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_AssetProductionOrder] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [DiplomaticContacts] (
        [Id] uniqueidentifier NOT NULL,
        [CivilizationId] uniqueidentifier NOT NULL,
        [ContactedCivilizationId] uniqueidentifier NOT NULL,
        [Status] int NOT NULL,
        [DiscoveredAtUtc] datetime2 NOT NULL,
        [Source] nvarchar(64) NOT NULL,
        CONSTRAINT [PK_DiplomaticContacts] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [exploration_knowledge] (
        [id] uniqueidentifier NOT NULL,
        [civilization_id] uniqueidentifier NOT NULL,
        [system_id] uniqueidentifier NOT NULL,
        [planet_id] uniqueidentifier NULL,
        [source] int NOT NULL,
        [source_mission_id] uniqueidentifier NULL,
        [discovered_at_utc] datetime2 NOT NULL,
        CONSTRAINT [PK_exploration_knowledge] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [ExplorationMissions] (
        [Id] uniqueidentifier NOT NULL,
        [CivilizationId] uniqueidentifier NOT NULL,
        [TargetSystemId] uniqueidentifier NOT NULL,
        [TargetPlanetId] uniqueidentifier NULL,
        [RequestedAtUtc] datetime2 NOT NULL,
        [DueAtUtc] datetime2 NOT NULL,
        [CompletedAtUtc] datetime2 NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_ExplorationMissions] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [galaxies] (
        [id] uniqueidentifier NOT NULL,
        [name] nvarchar(128) NOT NULL,
        CONSTRAINT [PK_galaxies] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [OrbitalAssetStock] (
        [Id] uniqueidentifier NOT NULL,
        [PlanetId] uniqueidentifier NOT NULL,
        [AssetType] int NOT NULL,
        [Quantity] int NOT NULL,
        CONSTRAINT [PK_OrbitalAssetStock] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [OrbitalGroup] (
        [Id] uniqueidentifier NOT NULL,
        [CivilizationId] uniqueidentifier NOT NULL,
        [OriginPlanetId] uniqueidentifier NOT NULL,
        [CurrentPlanetId] uniqueidentifier NOT NULL,
        [AssetType] int NOT NULL,
        [Quantity] int NOT NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_OrbitalGroup] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [OrbitalTransfers] (
        [Id] uniqueidentifier NOT NULL,
        [CivilizationId] uniqueidentifier NOT NULL,
        [OrbitalGroupId] uniqueidentifier NOT NULL,
        [OriginPlanetId] uniqueidentifier NOT NULL,
        [DestinationPlanetId] uniqueidentifier NOT NULL,
        [AbstractDistanceUnits] int NOT NULL,
        [DepartureAtUtc] datetime2 NOT NULL,
        [ArrivalAtUtc] datetime2 NOT NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_OrbitalTransfers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [planet_ownerships] (
        [id] uniqueidentifier NOT NULL,
        [planet_id] uniqueidentifier NOT NULL,
        [civilization_id] uniqueidentifier NOT NULL,
        [status] int NOT NULL,
        [claimed_at_utc] datetime2 NOT NULL,
        CONSTRAINT [PK_planet_ownerships] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [planet_production_profiles] (
        [id] uniqueidentifier NOT NULL,
        [planet_id] uniqueidentifier NOT NULL,
        [credits_per_hour] decimal(18,4) NOT NULL,
        [metal_per_hour] decimal(18,4) NOT NULL,
        [crystal_per_hour] decimal(18,4) NOT NULL,
        [gas_per_hour] decimal(18,4) NOT NULL,
        CONSTRAINT [PK_planet_production_profiles] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [planet_resource_stockpiles] (
        [id] uniqueidentifier NOT NULL,
        [planet_id] uniqueidentifier NOT NULL,
        [credits] decimal(18,4) NOT NULL,
        [metal] decimal(18,4) NOT NULL,
        [crystal] decimal(18,4) NOT NULL,
        [gas] decimal(18,4) NOT NULL,
        CONSTRAINT [PK_planet_resource_stockpiles] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [PlanetaryAssetStock] (
        [Id] uniqueidentifier NOT NULL,
        [PlanetId] uniqueidentifier NOT NULL,
        [AssetType] int NOT NULL,
        [Quantity] int NOT NULL,
        CONSTRAINT [PK_PlanetaryAssetStock] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [PlanetBuildingCapacities] (
        [Id] uniqueidentifier NOT NULL,
        [PlanetId] uniqueidentifier NOT NULL,
        [BaseCapacity] int NOT NULL,
        [BonusCapacity] int NOT NULL,
        CONSTRAINT [PK_PlanetBuildingCapacities] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [PlanetBuildings] (
        [Id] uniqueidentifier NOT NULL,
        [PlanetId] uniqueidentifier NOT NULL,
        [BuildingType] int NOT NULL,
        [Level] int NOT NULL,
        [Footprint] int NOT NULL,
        CONSTRAINT [PK_PlanetBuildings] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [PlanetConstructionOrders] (
        [Id] uniqueidentifier NOT NULL,
        [PlanetId] uniqueidentifier NOT NULL,
        [Action] int NOT NULL,
        [BuildingType] int NOT NULL,
        [TargetLevel] int NOT NULL,
        [Sequence] int NOT NULL,
        [StartsAtUtc] datetime2 NOT NULL,
        [EndsAtUtc] datetime2 NOT NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_PlanetConstructionOrders] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [PlanetPopulationProfiles] (
        [Id] uniqueidentifier NOT NULL,
        [PlanetId] uniqueidentifier NOT NULL,
        [TotalPopulation] bigint NOT NULL,
        [BaseRecruitablePopulation] bigint NOT NULL,
        [BaseCrewCapacity] bigint NOT NULL,
        CONSTRAINT [PK_PlanetPopulationProfiles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [player_profiles] (
        [id] uniqueidentifier NOT NULL,
        [user_id] nvarchar(128) NOT NULL,
        [display_name] nvarchar(128) NOT NULL,
        [normalized_display_name] nvarchar(128) NOT NULL,
        CONSTRAINT [PK_player_profiles] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [ResearchOrders] (
        [Id] uniqueidentifier NOT NULL,
        [CivilizationId] uniqueidentifier NOT NULL,
        [SourcePlanetId] uniqueidentifier NOT NULL,
        [ResearchType] int NOT NULL,
        [TargetLevel] int NOT NULL,
        [Sequence] int NOT NULL,
        [StartsAtUtc] datetime2 NOT NULL,
        [EndsAtUtc] datetime2 NOT NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_ResearchOrders] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [ResearchProjects] (
        [Id] uniqueidentifier NOT NULL,
        [CivilizationId] uniqueidentifier NOT NULL,
        [ResearchType] nvarchar(100) NOT NULL,
        [Level] int NOT NULL,
        CONSTRAINT [PK_ResearchProjects] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [solar_systems] (
        [id] uniqueidentifier NOT NULL,
        [galaxy_id] uniqueidentifier NOT NULL,
        [name] nvarchar(128) NOT NULL,
        [coordinates_x] int NOT NULL,
        [coordinates_y] int NOT NULL,
        [coordinates_z] int NOT NULL,
        CONSTRAINT [PK_solar_systems] PRIMARY KEY ([id]),
        CONSTRAINT [FK_solar_systems_galaxies_galaxy_id] FOREIGN KEY ([galaxy_id]) REFERENCES [galaxies] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [civilizations] (
        [id] uniqueidentifier NOT NULL,
        [player_profile_id] uniqueidentifier NOT NULL,
        [name] nvarchar(128) NOT NULL,
        [normalized_name] nvarchar(128) NOT NULL,
        [archetype] int NOT NULL,
        [status] int NOT NULL,
        [home_planet_id] uniqueidentifier NULL,
        CONSTRAINT [PK_civilizations] PRIMARY KEY ([id]),
        CONSTRAINT [FK_civilizations_player_profiles_player_profile_id] FOREIGN KEY ([player_profile_id]) REFERENCES [player_profiles] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [planets] (
        [id] uniqueidentifier NOT NULL,
        [solar_system_id] uniqueidentifier NOT NULL,
        [name] nvarchar(128) NOT NULL,
        [orbital_slot] int NOT NULL,
        [planet_type] int NOT NULL,
        [size] int NOT NULL,
        [colonization_status] int NOT NULL,
        CONSTRAINT [PK_planets] PRIMARY KEY ([id]),
        CONSTRAINT [FK_planets_solar_systems_solar_system_id] FOREIGN KEY ([solar_system_id]) REFERENCES [solar_systems] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE TABLE [stars] (
        [id] uniqueidentifier NOT NULL,
        [solar_system_id] uniqueidentifier NOT NULL,
        [name] nvarchar(128) NOT NULL,
        [star_type] int NOT NULL,
        CONSTRAINT [PK_stars] PRIMARY KEY ([id]),
        CONSTRAINT [FK_stars_solar_systems_solar_system_id] FOREIGN KEY ([solar_system_id]) REFERENCES [solar_systems] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_AllianceMemberships_AllianceId] ON [AllianceMemberships] ([AllianceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AllianceMemberships_AllianceId_CivilizationId] ON [AllianceMemberships] ([AllianceId], [CivilizationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_AllianceMemberships_CivilizationId] ON [AllianceMemberships] ([CivilizationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_AllianceMemberships_CivilizationId_JoinedAtUtc_Id] ON [AllianceMemberships] ([CivilizationId], [JoinedAtUtc], [Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_AlliancePacts_SourceAllianceId_Status_CreatedAtUtc_Id] ON [AlliancePacts] ([SourceAllianceId], [Status], [CreatedAtUtc], [Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AlliancePacts_SourceAllianceId_TargetAllianceId_PactType] ON [AlliancePacts] ([SourceAllianceId], [TargetAllianceId], [PactType]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_AlliancePacts_TargetAllianceId_Status_CreatedAtUtc_Id] ON [AlliancePacts] ([TargetAllianceId], [Status], [CreatedAtUtc], [Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_Alliances_Name] ON [Alliances] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_Alliances_Status_CreatedAtUtc_Id] ON [Alliances] ([Status], [CreatedAtUtc], [Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Alliances_Tag] ON [Alliances] ([Tag]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_AssetProductionOrder_PlanetId] ON [AssetProductionOrder] ([PlanetId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AssetProductionOrder_PlanetId_Sequence] ON [AssetProductionOrder] ([PlanetId], [Sequence]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_AssetProductionOrder_PlanetId_Status] ON [AssetProductionOrder] ([PlanetId], [Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_AssetProductionOrder_Target_Status_EndsAtUtc_Sequence] ON [AssetProductionOrder] ([Target], [Status], [EndsAtUtc], [Sequence]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [ix_civilizations_normalized_name] ON [civilizations] ([normalized_name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE UNIQUE INDEX [ux_civilizations_profile_normalized_name] ON [civilizations] ([player_profile_id], [normalized_name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_DiplomaticContacts_CivilizationId] ON [DiplomaticContacts] ([CivilizationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DiplomaticContacts_CivilizationId_ContactedCivilizationId] ON [DiplomaticContacts] ([CivilizationId], [ContactedCivilizationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_DiplomaticContacts_ContactedCivilizationId] ON [DiplomaticContacts] ([ContactedCivilizationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_exploration_knowledge_civilization_id] ON [exploration_knowledge] ([civilization_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_exploration_knowledge_civilization_id_system_id] ON [exploration_knowledge] ([civilization_id], [system_id]) WHERE planet_id IS NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_exploration_knowledge_civilization_id_system_id_planet_id] ON [exploration_knowledge] ([civilization_id], [system_id], [planet_id]) WHERE planet_id IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_exploration_knowledge_planet_id] ON [exploration_knowledge] ([planet_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_exploration_knowledge_system_id] ON [exploration_knowledge] ([system_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_ExplorationMissions_CivilizationId] ON [ExplorationMissions] ([CivilizationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_ExplorationMissions_CivilizationId_Status_DueAtUtc] ON [ExplorationMissions] ([CivilizationId], [Status], [DueAtUtc]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_ExplorationMissions_TargetPlanetId] ON [ExplorationMissions] ([TargetPlanetId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_ExplorationMissions_TargetSystemId] ON [ExplorationMissions] ([TargetSystemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE UNIQUE INDEX [IX_OrbitalAssetStock_PlanetId_AssetType] ON [OrbitalAssetStock] ([PlanetId], [AssetType]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_OrbitalGroup_CivilizationId] ON [OrbitalGroup] ([CivilizationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_OrbitalGroup_CurrentPlanetId] ON [OrbitalGroup] ([CurrentPlanetId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_OrbitalGroup_CurrentPlanetId_Status] ON [OrbitalGroup] ([CurrentPlanetId], [Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_OrbitalGroup_OriginPlanetId] ON [OrbitalGroup] ([OriginPlanetId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_OrbitalTransfers_CivilizationId] ON [OrbitalTransfers] ([CivilizationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_OrbitalTransfers_DestinationPlanetId] ON [OrbitalTransfers] ([DestinationPlanetId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_OrbitalTransfers_OrbitalGroupId] ON [OrbitalTransfers] ([OrbitalGroupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_OrbitalTransfers_OriginPlanetId] ON [OrbitalTransfers] ([OriginPlanetId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_OrbitalTransfers_Status_ArrivalAtUtc] ON [OrbitalTransfers] ([Status], [ArrivalAtUtc]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [ix_planet_ownerships_civilization_status] ON [planet_ownerships] ([civilization_id], [status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE UNIQUE INDEX [ux_planet_ownerships_planet_id] ON [planet_ownerships] ([planet_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE UNIQUE INDEX [ux_planet_production_profiles_planet_id] ON [planet_production_profiles] ([planet_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE UNIQUE INDEX [ux_planet_resource_stockpiles_planet_id] ON [planet_resource_stockpiles] ([planet_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PlanetaryAssetStock_PlanetId_AssetType] ON [PlanetaryAssetStock] ([PlanetId], [AssetType]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PlanetBuildingCapacities_PlanetId] ON [PlanetBuildingCapacities] ([PlanetId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_PlanetBuildings_PlanetId] ON [PlanetBuildings] ([PlanetId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_PlanetConstructionOrders_PlanetId] ON [PlanetConstructionOrders] ([PlanetId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PlanetConstructionOrders_PlanetId_Sequence] ON [PlanetConstructionOrders] ([PlanetId], [Sequence]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_PlanetConstructionOrders_PlanetId_Status] ON [PlanetConstructionOrders] ([PlanetId], [Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_PlanetConstructionOrders_Status_EndsAtUtc_Sequence] ON [PlanetConstructionOrders] ([Status], [EndsAtUtc], [Sequence]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PlanetPopulationProfiles_PlanetId] ON [PlanetPopulationProfiles] ([PlanetId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE UNIQUE INDEX [ux_planets_solar_system_orbital_slot] ON [planets] ([solar_system_id], [orbital_slot]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE UNIQUE INDEX [ux_player_profiles_normalized_display_name] ON [player_profiles] ([normalized_display_name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE UNIQUE INDEX [ux_player_profiles_user_id] ON [player_profiles] ([user_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_ResearchOrders_CivilizationId] ON [ResearchOrders] ([CivilizationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ResearchOrders_CivilizationId_Sequence] ON [ResearchOrders] ([CivilizationId], [Sequence]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_ResearchOrders_CivilizationId_Status] ON [ResearchOrders] ([CivilizationId], [Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE INDEX [IX_ResearchOrders_Status_EndsAtUtc_Sequence] ON [ResearchOrders] ([Status], [EndsAtUtc], [Sequence]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ResearchProjects_CivilizationId_ResearchType] ON [ResearchProjects] ([CivilizationId], [ResearchType]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE UNIQUE INDEX [ux_solar_systems_galaxy_coordinates] ON [solar_systems] ([galaxy_id], [coordinates_x], [coordinates_y], [coordinates_z]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    CREATE UNIQUE INDEX [ux_stars_solar_system_id] ON [stars] ([solar_system_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706131610_SqlServerInitialBaseline'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260706131610_SqlServerInitialBaseline', N'8.0.27');
END;
GO

COMMIT;
GO


