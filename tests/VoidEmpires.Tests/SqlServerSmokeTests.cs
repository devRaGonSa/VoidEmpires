using Microsoft.Data.SqlClient;
using Xunit.Abstractions;

namespace VoidEmpires.Tests;

public sealed class SqlServerSmokeTests(ITestOutputHelper output)
{
    private const string EnabledVariableName = "VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED";
    private const string ConnectionStringVariableName = "VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING";

    [Fact]
    public async Task ReadOnlySelectOneRunsOnlyWhenExplicitlyEnabled()
    {
        if (!TryGetRequiredConnectionString(out var connectionString, out var skipReason))
        {
            output.WriteLine(skipReason);
            return;
        }

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1";

        var result = await command.ExecuteScalarAsync();

        output.WriteLine("SQL Server smoke coverage enabled through environment variables.");
        Assert.Equal(1, Convert.ToInt32(result));
    }

    private static bool TryGetRequiredConnectionString(out string connectionString, out string skipReason)
    {
        connectionString = string.Empty;
        var enabled = Environment.GetEnvironmentVariable(EnabledVariableName);
        if (!string.Equals(enabled, "true", StringComparison.OrdinalIgnoreCase))
        {
            skipReason = $"SQL Server smoke test skipped. Set {EnabledVariableName}=true to enable it.";
            return false;
        }

        connectionString = Environment.GetEnvironmentVariable(ConnectionStringVariableName)?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            skipReason = $"SQL Server smoke test skipped. Set {ConnectionStringVariableName} outside source control.";
            return false;
        }

        skipReason = string.Empty;
        return true;
    }
}
