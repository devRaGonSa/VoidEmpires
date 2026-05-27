using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Identity;
using VoidEmpires.Infrastructure.Identity;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure;

public static class VoidEmpiresPersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddVoidEmpiresPersistence(
        this IServiceCollection services,
        string? connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return services;
        }

        services.AddDbContext<VoidEmpiresDbContext>(options => options.UseNpgsql(connectionString));

        return services;
    }

    public static IServiceCollection AddVoidEmpiresIdentity(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddIdentityCore<VoidEmpiresUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<VoidEmpiresDbContext>()
            .AddTokenProvider<DataProtectorTokenProvider<VoidEmpiresUser>>(TokenOptions.DefaultProvider);

        services.AddScoped<IdentityAccountService>();
        services.AddScoped<IUserRegistrationService>(provider => provider.GetRequiredService<IdentityAccountService>());
        services.AddScoped<IEmailConfirmationService>(provider => provider.GetRequiredService<IdentityAccountService>());

        return services;
    }
}
