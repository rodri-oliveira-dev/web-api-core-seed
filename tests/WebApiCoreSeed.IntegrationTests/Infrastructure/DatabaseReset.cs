using Microsoft.Data.SqlClient;

namespace WebApiCoreSeed.IntegrationTests.Infrastructure;

public sealed class DatabaseReset
{
    private static readonly string[] DeleteStatements =
    [
        "DELETE FROM [AspNetUserTokens];",
        "DELETE FROM [AspNetUserRoles];",
        "DELETE FROM [AspNetUserLogins];",
        "DELETE FROM [AspNetUserClaims];",
        "DELETE FROM [AspNetRoleClaims];",
        "DELETE FROM [AspNetRoles];",
        "DELETE FROM [AspNetUsers];",
        "DELETE FROM [PedidoPrato];",
        "DELETE FROM [Pedidos];",
        "DELETE FROM [Pratos];",
        "DELETE FROM [Atendentes];",
        "DELETE FROM [Mesas];",
        "DELETE FROM [Loggin];"
    ];

    private readonly string _connectionString;

    public DatabaseReset(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task ResetAsync()
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        foreach (var statement in DeleteStatements)
        {
            await using var command = new SqlCommand(statement, connection, (SqlTransaction)transaction);
            await command.ExecuteNonQueryAsync();
        }

        await transaction.CommitAsync();
    }
}
