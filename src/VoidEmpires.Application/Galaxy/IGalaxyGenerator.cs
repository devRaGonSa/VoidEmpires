namespace VoidEmpires.Application.Galaxy;

public interface IGalaxyGenerator
{
    GeneratedGalaxyResult Generate(GenerateGalaxyRequest request);
}
