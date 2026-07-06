using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VoidEmpires.Infrastructure.Persistence;

public sealed class SqlServerDesignTimeMigrationsAssembly : IMigrationsAssembly
{
    private const string SqlServerMigrationsNamespace = "VoidEmpires.Infrastructure.Persistence.Migrations.SqlServer";

    private readonly Type _contextType;
    private IReadOnlyDictionary<string, TypeInfo>? _migrations;
    private ModelSnapshot? _modelSnapshot;
    private bool _modelSnapshotInitialized;

    public SqlServerDesignTimeMigrationsAssembly(ICurrentDbContext currentDbContext)
    {
        _contextType = currentDbContext.Context.GetType();
    }

    public IReadOnlyDictionary<string, TypeInfo> Migrations =>
        _migrations ??= Assembly.DefinedTypes
            .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Migration)))
            .Select(type => new
            {
                Type = type,
                Migration = type.GetCustomAttribute<MigrationAttribute>(),
                DbContext = type.GetCustomAttribute<DbContextAttribute>()
            })
            .Where(candidate =>
                candidate.Migration is not null
                && IsSqlServerMigrationType(candidate.Type)
                && IsCurrentContext(candidate.DbContext))
            .OrderBy(candidate => candidate.Migration!.Id, StringComparer.Ordinal)
            .ToDictionary(
                candidate => candidate.Migration!.Id,
                candidate => candidate.Type,
                StringComparer.OrdinalIgnoreCase);

    public ModelSnapshot? ModelSnapshot
    {
        get
        {
            if (_modelSnapshotInitialized)
            {
                return _modelSnapshot;
            }

            _modelSnapshot = Assembly.DefinedTypes
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(ModelSnapshot)))
                .Where(IsSqlServerMigrationType)
                .Where(type => IsCurrentContext(type.GetCustomAttribute<DbContextAttribute>()))
                .Select(type => (ModelSnapshot?)Activator.CreateInstance(type.AsType()))
                .FirstOrDefault(snapshot => snapshot is not null);

            _modelSnapshotInitialized = true;

            return _modelSnapshot;
        }
    }

    public Assembly Assembly => typeof(VoidEmpiresDbContext).Assembly;

    public string? FindMigrationId(string nameOrId) =>
        Migrations.Keys.FirstOrDefault(id =>
            string.Equals(id, nameOrId, StringComparison.OrdinalIgnoreCase)
            || string.Equals(GetMigrationName(id), nameOrId, StringComparison.OrdinalIgnoreCase));

    public Migration CreateMigration(TypeInfo migrationClass, string activeProvider)
    {
        var migration = (Migration?)Activator.CreateInstance(migrationClass.AsType())
            ?? throw new InvalidOperationException($"Unable to create migration '{migrationClass.FullName}'.");

        migration.ActiveProvider = activeProvider;

        return migration;
    }

    private static bool IsSqlServerMigrationType(TypeInfo type) =>
        type.Namespace?.StartsWith(SqlServerMigrationsNamespace, StringComparison.Ordinal) == true;

    private bool IsCurrentContext(DbContextAttribute? dbContextAttribute) =>
        dbContextAttribute?.ContextType == null || dbContextAttribute.ContextType == _contextType;

    private static string GetMigrationName(string migrationId)
    {
        var separatorIndex = migrationId.IndexOf('_', StringComparison.Ordinal);

        return separatorIndex < 0 || separatorIndex == migrationId.Length - 1
            ? migrationId
            : migrationId[(separatorIndex + 1)..];
    }
}
