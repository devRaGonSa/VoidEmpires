using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VoidEmpires.Infrastructure.Persistence;

public sealed class VoidEmpiresDbContextFactory : IDesignTimeDbContextFactory<VoidEmpiresDbContext>
{
    public VoidEmpiresDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseNpgsql("Host=localhost;Database=voidempires_design")
            .Options;

        return new VoidEmpiresDbContext(options);
    }
}
