using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VoidEmpires.Infrastructure.Identity;

namespace VoidEmpires.Infrastructure.Identity
{
    public sealed class VoidEmpiresUser : IdentityUser
    {
    }
}

namespace VoidEmpires.Infrastructure.Persistence
{
    public sealed class VoidEmpiresDbContext : IdentityDbContext<VoidEmpiresUser>
    {
        public VoidEmpiresDbContext(DbContextOptions<VoidEmpiresDbContext> options)
            : base(options)
        {
        }
    }
}
