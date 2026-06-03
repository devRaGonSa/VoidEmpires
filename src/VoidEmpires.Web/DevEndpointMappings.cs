using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using VoidEmpires.Application.Development;
using VoidEmpires.Application.Assets;
using VoidEmpires.Application.Buildings;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Application.Galaxy;
using VoidEmpires.Application.Players;
using VoidEmpires.Application.Research;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Players;
using VoidEmpires.Domain.Research;

internal static class DevEndpointMappings
{
    public static void MapDevEndpointMappings(this WebApplication app)
    {
        app.MapPost("/api/dev/seeds/apply", async (
            ApplyDevelopmentSeedApiRequest request,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (!IsPersistenceConfigured(configuration))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            var errors = ValidateApplyDevelopmentSeed(request);
            if (errors.Count > 0)
            {
                return Results.BadRequest(new ApplyDevelopmentSeedApiResponse(false, request.Profile, [], errors));
            }

            var service = services.GetRequiredService<IDevelopmentSeedService>();
            var result = await service.ApplyAsync(new ApplyDevelopmentSeedRequest(request.Profile!.Trim()), cancellationToken);
            var response = new ApplyDevelopmentSeedApiResponse(
                result.Succeeded,
                result.Profile,
                result.AppliedSteps,
                result.Errors);

            return result.Succeeded
                ? Results.Ok(response)
                : Results.BadRequest(response);
        });

        app.MapPost("/api/dev/galaxies/generate", async (
            GenerateGalaxyApiRequest request,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (!IsPersistenceConfigured(configuration))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            var errors = ValidateGalaxyGeneration(request);
            if (errors.Count > 0)
            {
                return Results.BadRequest(new GalaxyGenerationApiResponse(false, null, null, 0, 0, errors));
            }

            var generationService = services.GetRequiredService<IGalaxyGenerationService>();
            var result = await generationService.GenerateAndPersistAsync(new GenerateAndPersistGalaxyRequest(
                request.Name!,
                request.Seed!,
                request.SolarSystemCount,
                request.MinPlanetsPerSystem,
                request.MaxPlanetsPerSystem,
                request.OverwriteExisting), cancellationToken);

            var response = new GalaxyGenerationApiResponse(
                result.Succeeded,
                result.GalaxyId,
                result.GalaxyName,
                result.SolarSystemCount,
                result.PlanetCount,
                result.Errors);

            return result.Succeeded
                ? Results.Created($"/api/dev/galaxies/{result.GalaxyId}", response)
                : Results.Conflict(response);
        });

        app.MapPost("/api/dev/players/starting-civilization", async (
            CreateStartingCivilizationApiRequest request,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (!IsPersistenceConfigured(configuration))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            var errors = ValidateStartingCivilization(request);
            if (errors.Count > 0)
            {
                return Results.BadRequest(new StartingCivilizationApiResponse(false, null, null, null, errors));
            }

            var service = services.GetRequiredService<IStartingCivilizationService>();
            var result = await service.CreateAsync(new CreateStartingCivilizationRequest(
                request.UserId!,
                request.DisplayName!,
                request.CivilizationName!,
                request.Archetype,
                request.HomePlanetId), cancellationToken);

            var response = new StartingCivilizationApiResponse(
                result.Succeeded,
                result.PlayerProfileId,
                result.CivilizationId,
                result.HomePlanetId,
                result.Errors);

            return result.Succeeded
                ? Results.Created($"/api/dev/players/{result.PlayerProfileId}/civilizations/{result.CivilizationId}", response)
                : Results.Conflict(response);
        });

        app.MapPost("/api/dev/buildings/construction-orders/enqueue", async (
            EnqueueConstructionOrderApiRequest request,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (!IsPersistenceConfigured(configuration))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            var errors = ValidateEnqueueConstructionOrder(request);
            if (errors.Count > 0)
            {
                return Results.BadRequest(new EnqueueConstructionOrderApiResponse(false, null, null, null, errors));
            }

            var service = services.GetRequiredService<IPlanetConstructionQueueService>();
            var result = await service.EnqueueAsync(new EnqueueConstructionOrderRequest(
                request.PlanetId!.Value,
                request.CivilizationId!.Value,
                request.Action!.Value,
                request.BuildingType!.Value,
                request.RequestedAtUtc!.Value), cancellationToken);

            var response = new EnqueueConstructionOrderApiResponse(
                result.Succeeded,
                result.OrderId,
                result.StartsAtUtc,
                result.EndsAtUtc,
                result.Errors);

            return result.Succeeded
                ? Results.Created($"/api/dev/buildings/construction-orders/{result.OrderId}", response)
                : Results.Conflict(response);
        });

        app.MapPost("/api/dev/buildings/construction-orders/complete-due", async (
            CompleteConstructionOrdersApiRequest request,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (!IsPersistenceConfigured(configuration))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            var errors = ValidateCompleteConstructionOrders(request);
            if (errors.Count > 0)
            {
                return Results.BadRequest(new CompleteConstructionOrdersApiResponse(false, 0, [], errors));
            }

            var service = services.GetRequiredService<IConstructionOrderCompletionService>();
            var result = await service.CompleteDueOrdersAsync(request.NowUtc!.Value, cancellationToken);

            return Results.Ok(new CompleteConstructionOrdersApiResponse(
                true,
                result.CompletedCount,
                result.CompletedOrderIds,
                []));
        });

        app.MapPost("/api/dev/assets/production/enqueue", async (
            EnqueueAssetProductionApiRequest request,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (!IsPersistenceConfigured(configuration))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            var errors = ValidateEnqueueAssetProduction(request);
            if (errors.Count > 0)
            {
                return Results.BadRequest(new EnqueueAssetProductionApiResponse(false, null, null, null, errors));
            }

            var service = services.GetRequiredService<IAssetProductionQueueService>();
            var result = await service.EnqueueAsync(new EnqueueAssetProductionRequest(
                request.PlanetId!.Value,
                request.Target!.Value,
                request.PlanetaryAssetType,
                request.SpaceAssetType,
                request.Quantity!.Value,
                request.RequestedAtUtc!.Value), cancellationToken);

            var response = new EnqueueAssetProductionApiResponse(
                result.Succeeded,
                result.OrderId,
                result.StartsAtUtc,
                result.EndsAtUtc,
                result.Errors);

            return result.Succeeded
                ? Results.Created($"/api/dev/assets/production/{result.OrderId}", response)
                : Results.Conflict(response);
        });

        app.MapPost("/api/dev/assets/production/process-due", async (
            ProcessAssetProductionApiRequest request,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (!IsPersistenceConfigured(configuration))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            var errors = ValidateProcessAssetProduction(request);
            if (errors.Count > 0)
            {
                return Results.BadRequest(new ProcessAssetProductionApiResponse(false, 0, [], errors));
            }

            var service = services.GetRequiredService<IAssetOrderProcessor>();
            var result = await service.ProcessAsync(request.NowUtc!.Value, cancellationToken);

            return Results.Ok(new ProcessAssetProductionApiResponse(
                true,
                result.CompletedCount,
                result.CompletedOrderIds,
                []));
        });

        app.MapGet("/api/dev/shipyard/ui-state", async (
            Guid? civilizationId,
            Guid? planetId,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (!IsPersistenceConfigured(configuration))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (civilizationId is null || civilizationId == Guid.Empty)
            {
                return Results.BadRequest(new DevShipyardUiStateApiResponse(false, null, ["Civilization id is required."]));
            }

            var service = services.GetRequiredService<IDevShipyardUiStateService>();
            var uiState = await service.GetAsync(
                new GetDevShipyardUiStateRequest(civilizationId.Value, planetId),
                cancellationToken);

            if (uiState.Shipyard is null && uiState.Errors.Count > 0)
            {
                return Results.NotFound(new DevShipyardUiStateApiResponse(false, null, uiState.Errors));
            }

            return Results.Ok(new DevShipyardUiStateApiResponse(true, uiState, []));
        });

        app.MapPost("/api/dev/research/orders/enqueue", async (
            EnqueueResearchOrderApiRequest request,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (!IsPersistenceConfigured(configuration))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            var errors = ValidateEnqueueResearchOrder(request);
            if (errors.Count > 0)
            {
                return Results.BadRequest(new EnqueueResearchOrderApiResponse(false, null, null, null, errors));
            }

            var service = services.GetRequiredService<IResearchQueueService>();
            var result = await service.EnqueueAsync(new EnqueueResearchOrderRequest(
                request.CivilizationId!.Value,
                request.SourcePlanetId!.Value,
                request.ResearchType!.Value,
                request.RequestedAtUtc!.Value), cancellationToken);

            var response = new EnqueueResearchOrderApiResponse(
                result.Succeeded,
                result.OrderId,
                result.StartsAtUtc,
                result.EndsAtUtc,
                result.Errors);

            return result.Succeeded
                ? Results.Created($"/api/dev/research/orders/{result.OrderId}", response)
                : Results.Conflict(response);
        });

        app.MapPost("/api/dev/research/orders/complete-due", async (
            CompleteResearchOrdersApiRequest request,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (!IsPersistenceConfigured(configuration))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            var errors = ValidateCompleteResearchOrders(request);
            if (errors.Count > 0)
            {
                return Results.BadRequest(new CompleteResearchOrdersApiResponse(false, 0, [], errors));
            }

            var service = services.GetRequiredService<IResearchOrderCompletionService>();
            var result = await service.CompleteDueOrdersAsync(request.NowUtc!.Value, cancellationToken);

            return Results.Ok(new CompleteResearchOrdersApiResponse(
                true,
                result.CompletedCount,
                result.CompletedOrderIds,
                []));
        });

        app.MapPost("/api/dev/fleets/orbital-groups/create-from-stock", async (
            CreateOrbitalGroupApiRequest request,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (!IsPersistenceConfigured(configuration))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            var errors = ValidateCreateOrbitalGroup(request);
            if (errors.Count > 0)
            {
                return Results.BadRequest(new CreateOrbitalGroupApiResponse(false, null, errors));
            }

            var service = services.GetRequiredService<IOrbitalGroupService>();
            var result = await service.CreateFromLocalStockAsync(new CreateOrbitalGroupRequest(
                request.CivilizationId!.Value,
                request.OriginPlanetId!.Value,
                request.CurrentPlanetId!.Value,
                request.AssetType!.Value,
                request.Quantity!.Value), cancellationToken);

            var response = new CreateOrbitalGroupApiResponse(
                result.Succeeded,
                result.OrbitalGroupId,
                result.Errors);

            return result.Succeeded
                ? Results.Created($"/api/dev/fleets/orbital-groups/{result.OrbitalGroupId}", response)
                : Results.Conflict(response);
        });

        app.MapDevOrbitalGroupLookupEndpoints();
        app.MapDevOrbitalGroupSplitEndpoints();
        app.MapDevOrbitalGroupMergeEndpoints();
        app.MapDevOrbitalTravelEstimateEndpoints();
        app.MapDevOrbitalTransferCreationEndpoints();
        app.MapDevOrbitalTransferCancelEndpoints();
        app.MapDevOrbitalTransferCompletionEndpoints();
        app.MapDevOrbitalTransferLookupEndpoints();
        app.MapDevFleetOperationalOverviewEndpoints();
        app.MapDevPlanetVisualStateEndpoints();
    }

    private static bool IsPersistenceConfigured(IConfiguration configuration) =>
        !string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection"));

    private static IReadOnlyList<string> ValidateGalaxyGeneration(GenerateGalaxyApiRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors.Add("Galaxy name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Seed))
        {
            errors.Add("Galaxy seed is required.");
        }

        if (request.SolarSystemCount <= 0)
        {
            errors.Add("Solar system count must be positive.");
        }

        if (request.MinPlanetsPerSystem <= 0)
        {
            errors.Add("Minimum planets per system must be positive.");
        }

        if (request.MaxPlanetsPerSystem < request.MinPlanetsPerSystem)
        {
            errors.Add("Maximum planets per system must be greater than or equal to the minimum.");
        }

        return errors;
    }

    private static IReadOnlyList<string> ValidateApplyDevelopmentSeed(ApplyDevelopmentSeedApiRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Profile))
        {
            errors.Add("Seed profile is required.");
        }

        return errors;
    }

    private static IReadOnlyList<string> ValidateStartingCivilization(CreateStartingCivilizationApiRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            errors.Add("User id is required.");
        }

        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            errors.Add("Display name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.CivilizationName))
        {
            errors.Add("Civilization name is required.");
        }

        return errors;
    }

    private static IReadOnlyList<string> ValidateEnqueueConstructionOrder(EnqueueConstructionOrderApiRequest request)
    {
        var errors = new List<string>();

        if (request.PlanetId is null || request.PlanetId == Guid.Empty)
        {
            errors.Add("Planet id is required.");
        }

        if (request.CivilizationId is null || request.CivilizationId == Guid.Empty)
        {
            errors.Add("Civilization id is required.");
        }

        if (request.Action is null)
        {
            errors.Add("Construction action is required.");
        }

        if (request.BuildingType is null)
        {
            errors.Add("Building type is required.");
        }

        if (request.RequestedAtUtc is null)
        {
            errors.Add("Requested date is required.");
        }
        else if (request.RequestedAtUtc.Value.Kind != DateTimeKind.Utc)
        {
            errors.Add("Requested date must be UTC.");
        }

        return errors;
    }

    private static IReadOnlyList<string> ValidateCompleteConstructionOrders(CompleteConstructionOrdersApiRequest request)
    {
        var errors = new List<string>();

        if (request.NowUtc is null)
        {
            errors.Add("Current date is required.");
        }
        else if (request.NowUtc.Value.Kind != DateTimeKind.Utc)
        {
            errors.Add("Current date must be UTC.");
        }

        return errors;
    }

    private static IReadOnlyList<string> ValidateEnqueueAssetProduction(EnqueueAssetProductionApiRequest request)
    {
        var errors = new List<string>();

        if (request.PlanetId is null || request.PlanetId == Guid.Empty)
        {
            errors.Add("Planet id is required.");
        }

        if (request.Target is null)
        {
            errors.Add("Asset production target is required.");
        }

        if (request.Target == AssetProductionTarget.Planetary && request.PlanetaryAssetType is null)
        {
            errors.Add("Planetary asset type is required.");
        }

        if (request.Target == AssetProductionTarget.Orbital && request.SpaceAssetType is null)
        {
            errors.Add("Space asset type is required.");
        }

        if (request.Quantity is null || request.Quantity <= 0)
        {
            errors.Add("Quantity must be positive.");
        }

        if (request.RequestedAtUtc is null)
        {
            errors.Add("Requested date is required.");
        }
        else if (request.RequestedAtUtc.Value.Kind != DateTimeKind.Utc)
        {
            errors.Add("Requested date must be UTC.");
        }

        return errors;
    }

    private static IReadOnlyList<string> ValidateProcessAssetProduction(ProcessAssetProductionApiRequest request)
    {
        var errors = new List<string>();

        if (request.NowUtc is null)
        {
            errors.Add("Current date is required.");
        }
        else if (request.NowUtc.Value.Kind != DateTimeKind.Utc)
        {
            errors.Add("Current date must be UTC.");
        }

        return errors;
    }

    private static IReadOnlyList<string> ValidateEnqueueResearchOrder(EnqueueResearchOrderApiRequest request)
    {
        var errors = new List<string>();

        if (request.CivilizationId is null || request.CivilizationId == Guid.Empty)
        {
            errors.Add("Civilization id is required.");
        }

        if (request.SourcePlanetId is null || request.SourcePlanetId == Guid.Empty)
        {
            errors.Add("Source planet id is required.");
        }

        if (request.ResearchType is null)
        {
            errors.Add("Research type is required.");
        }

        if (request.RequestedAtUtc is null)
        {
            errors.Add("Requested date is required.");
        }
        else if (request.RequestedAtUtc.Value.Kind != DateTimeKind.Utc)
        {
            errors.Add("Requested date must be UTC.");
        }

        return errors;
    }

    private static IReadOnlyList<string> ValidateCompleteResearchOrders(CompleteResearchOrdersApiRequest request)
    {
        var errors = new List<string>();

        if (request.NowUtc is null)
        {
            errors.Add("Current date is required.");
        }
        else if (request.NowUtc.Value.Kind != DateTimeKind.Utc)
        {
            errors.Add("Current date must be UTC.");
        }

        return errors;
    }

    private static IReadOnlyList<string> ValidateCreateOrbitalGroup(CreateOrbitalGroupApiRequest request)
    {
        var errors = new List<string>();

        if (request.CivilizationId is null || request.CivilizationId == Guid.Empty)
        {
            errors.Add("Civilization id is required.");
        }

        if (request.OriginPlanetId is null || request.OriginPlanetId == Guid.Empty)
        {
            errors.Add("Origin planet id is required.");
        }

        if (request.CurrentPlanetId is null || request.CurrentPlanetId == Guid.Empty)
        {
            errors.Add("Current planet id is required.");
        }

        if (request.AssetType is null)
        {
            errors.Add("Space asset type is required.");
        }

        if (request.Quantity is null || request.Quantity <= 0)
        {
            errors.Add("Quantity must be positive.");
        }

        return errors;
    }
}

internal sealed record ApplyDevelopmentSeedApiRequest(string? Profile);

internal sealed record ApplyDevelopmentSeedApiResponse(
    bool Succeeded,
    string? Profile,
    IReadOnlyList<string> AppliedSteps,
    IReadOnlyList<string> Errors);

internal sealed record GenerateGalaxyApiRequest(
    string? Name,
    string? Seed,
    int SolarSystemCount,
    int MinPlanetsPerSystem,
    int MaxPlanetsPerSystem,
    bool OverwriteExisting = false);

internal sealed record GalaxyGenerationApiResponse(
    bool Succeeded,
    Guid? GalaxyId,
    string? GalaxyName,
    int SolarSystemCount,
    int PlanetCount,
    IReadOnlyList<string> Errors);

internal sealed record CreateStartingCivilizationApiRequest(
    string? UserId,
    string? DisplayName,
    string? CivilizationName,
    CivilizationArchetype Archetype,
    Guid? HomePlanetId = null);

internal sealed record StartingCivilizationApiResponse(
    bool Succeeded,
    Guid? PlayerProfileId,
    Guid? CivilizationId,
    Guid? HomePlanetId,
    IReadOnlyList<string> Errors);

internal sealed record EnqueueConstructionOrderApiRequest(
    Guid? PlanetId,
    Guid? CivilizationId,
    ConstructionQueueItemAction? Action,
    BuildingType? BuildingType,
    DateTime? RequestedAtUtc);

internal sealed record EnqueueConstructionOrderApiResponse(
    bool Succeeded,
    Guid? OrderId,
    DateTime? StartsAtUtc,
    DateTime? EndsAtUtc,
    IReadOnlyList<string> Errors);

internal sealed record CompleteConstructionOrdersApiRequest(DateTime? NowUtc);

internal sealed record CompleteConstructionOrdersApiResponse(
    bool Succeeded,
    int CompletedCount,
    IReadOnlyList<Guid> CompletedOrderIds,
    IReadOnlyList<string> Errors);

internal sealed record EnqueueAssetProductionApiRequest(
    Guid? PlanetId,
    AssetProductionTarget? Target,
    PlanetaryAssetType? PlanetaryAssetType,
    SpaceAssetType? SpaceAssetType,
    int? Quantity,
    DateTime? RequestedAtUtc);

internal sealed record EnqueueAssetProductionApiResponse(
    bool Succeeded,
    Guid? OrderId,
    DateTime? StartsAtUtc,
    DateTime? EndsAtUtc,
    IReadOnlyList<string> Errors);

internal sealed record ProcessAssetProductionApiRequest(DateTime? NowUtc);

internal sealed record ProcessAssetProductionApiResponse(
    bool Succeeded,
    int CompletedCount,
    IReadOnlyList<Guid> CompletedOrderIds,
    IReadOnlyList<string> Errors);

internal sealed record DevShipyardUiStateApiResponse(
    bool Succeeded,
    GetDevShipyardUiStateResult? UiState,
    IReadOnlyList<string> Errors);

internal sealed record EnqueueResearchOrderApiRequest(
    Guid? CivilizationId,
    Guid? SourcePlanetId,
    [property: JsonConverter(typeof(JsonStringEnumConverter<ResearchType>))]
    ResearchType? ResearchType,
    DateTime? RequestedAtUtc);

internal sealed record EnqueueResearchOrderApiResponse(
    bool Succeeded,
    Guid? OrderId,
    DateTime? StartsAtUtc,
    DateTime? EndsAtUtc,
    IReadOnlyList<string> Errors);

internal sealed record CompleteResearchOrdersApiRequest(DateTime? NowUtc);

internal sealed record CompleteResearchOrdersApiResponse(
    bool Succeeded,
    int CompletedCount,
    IReadOnlyList<Guid> CompletedOrderIds,
    IReadOnlyList<string> Errors);

internal sealed record CreateOrbitalGroupApiRequest(
    Guid? CivilizationId,
    Guid? OriginPlanetId,
    Guid? CurrentPlanetId,
    SpaceAssetType? AssetType,
    int? Quantity);

internal sealed record CreateOrbitalGroupApiResponse(
    bool Succeeded,
    Guid? OrbitalGroupId,
    IReadOnlyList<string> Errors);
