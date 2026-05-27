using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Email;
using VoidEmpires.Infrastructure.Email;

namespace VoidEmpires.Infrastructure;

public static class VoidEmpiresEmailServiceCollectionExtensions
{
    public static IServiceCollection AddVoidEmpiresTransactionalEmail(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHttpClient<ITransactionalEmailSender, BrevoTransactionalEmailSender>();

        return services;
    }
}
