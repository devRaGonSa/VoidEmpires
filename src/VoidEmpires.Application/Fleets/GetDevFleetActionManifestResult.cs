namespace VoidEmpires.Application.Fleets;

public sealed record GetDevFleetActionManifestResult(
    IReadOnlyList<DevFleetActionManifestItem> Actions);

public sealed record DevFleetActionManifestItem(
    string ActionKey,
    string DisplayName,
    string Method,
    string Route,
    bool IsReadOnly,
    IReadOnlyList<DevFleetActionFieldDto> RequiredFields,
    int SuccessStatus,
    IReadOnlyList<int> ErrorStatuses,
    string Notes);

public sealed record DevFleetActionFieldDto(
    string Name,
    string Type,
    bool IsRequired);
