using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Application.Players;

public interface IHomePlanetAllocator
{
    Task<Planet> AllocateAsync(
        string homePlanetName,
        CancellationToken cancellationToken = default);
}
