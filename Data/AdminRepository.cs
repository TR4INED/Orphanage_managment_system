using Microsoft.Data.SqlClient;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Data;

public sealed class AdminRepository
{
    private readonly string _connectionString;

    public AdminRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("OrphanageDb")
            ?? throw new InvalidOperationException("Missing connection string: ConnectionStrings:OrphanageDb");
    }

    public async Task<IReadOnlyList<AdminRecord>> GetAdminsAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<AdminRecord>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT A_ID, A_Pno, A_name
FROM Admin
ORDER BY A_ID;";

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new AdminRecord
            {
                A_ID = reader.GetInt32(reader.GetOrdinal("A_ID")),
                A_Pno = reader.GetString(reader.GetOrdinal("A_Pno")),
                A_name = reader.GetString(reader.GetOrdinal("A_name")),
            });
        }

        return results;
    }
}
