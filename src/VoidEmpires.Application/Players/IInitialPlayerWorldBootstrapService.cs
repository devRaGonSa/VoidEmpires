namespace VoidEmpires.Application.Players;

public interface IInitialPlayerWorldBootstrapService
{
    Task<InitialPlayerWorldBootstrapResult> CreateAsync(
        InitialPlayerWorldBootstrapRequest request,
        CancellationToken cancellationToken = default);
}
