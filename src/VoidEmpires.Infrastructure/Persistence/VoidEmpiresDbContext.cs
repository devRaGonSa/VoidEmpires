using Microsoft.EntityFrameworkCore;

namespace VoidEmpires.Infrastructure.Persistence;

public sealed class VoidEmpiresDbContext : DbContext
{
    public VoidEmpiresDbContext(DbContextOptions<VoidEmpiresDbContext> options)
        : base(options)
    {
    }
}
