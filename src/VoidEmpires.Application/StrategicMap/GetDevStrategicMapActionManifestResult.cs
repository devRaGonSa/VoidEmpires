namespace VoidEmpires.Application.StrategicMap;

public sealed record GetDevStrategicMapActionManifestResult(
    IReadOnlyList<DevStrategicMapActionManifestItem> Actions);

public sealed record DevStrategicMapActionManifestItem(
    string ActionKey,
    string DisplayName,
    string Method,
    string Route,
    bool IsReadOnly,
    IReadOnlyList<DevStrategicMapActionFieldDto> RequiredFields,
    int SuccessStatus,
    IReadOnlyList<int> ErrorStatuses,
    string Notes);

public sealed record DevStrategicMapActionFieldDto(
    string Name,
    string Type,
    bool IsRequired);
