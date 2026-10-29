using Microsoft.Data.SqlClient;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Data;

public sealed class VolunteerRepository
{
    private readonly string _connectionString;

    public VolunteerRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("OrphanageDb")
            ?? throw new InvalidOperationException("Missing connection string: ConnectionStrings:OrphanageDb");
    }

    public async Task<IReadOnlyList<Volunteer>> GetVolunteersAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<Volunteer>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT v.V_ID, v.V_name, v.V_hours, COUNT(ab.O_ID) AS AssistedCount
FROM Volunteer v
LEFT JOIN Assisted_by ab ON ab.V_ID = v.V_ID
GROUP BY v.V_ID, v.V_name, v.V_hours
ORDER BY v.V_ID;";

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new Volunteer
            {
                V_ID = reader.GetInt32(reader.GetOrdinal("V_ID")),
                V_name = reader.GetString(reader.GetOrdinal("V_name")),
                V_hours = reader.GetInt32(reader.GetOrdinal("V_hours")),
                AssistedCount = reader.GetInt32(reader.GetOrdinal("AssistedCount")),
            });
        }

        return results;
    }

    public async Task<int> CreateVolunteerAsync(string name, int hours, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
INSERT INTO Volunteer (V_name, V_hours)
OUTPUT INSERTED.V_ID
VALUES (@Name, @Hours);";

        cmd.Parameters.AddWithValue("@Name", name);
        cmd.Parameters.AddWithValue("@Hours", hours);

        var newIdObj = await cmd.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(newIdObj);
    }

    public async Task CreateAssistedByAsync(int volunteerId, int orphanId, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
INSERT INTO Assisted_by (V_ID, O_ID)
VALUES (@VId, @OId);";

        cmd.Parameters.AddWithValue("@VId", volunteerId);
        cmd.Parameters.AddWithValue("@OId", orphanId);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }
}
