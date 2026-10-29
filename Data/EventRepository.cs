using Microsoft.Data.SqlClient;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Data;

public sealed class EventRepository
{
    private readonly string _connectionString;

    public EventRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("OrphanageDb")
            ?? throw new InvalidOperationException("Missing connection string: ConnectionStrings:OrphanageDb");
    }

    public async Task<IReadOnlyList<EventRecord>> GetEventsAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<EventRecord>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT E_ID, E_Date, E_participants, E_Location, A_ID, O_ID
FROM Events
ORDER BY E_ID;";

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var date = reader.GetDateTime(reader.GetOrdinal("E_Date"));

            results.Add(new EventRecord
            {
                E_ID = reader.GetInt32(reader.GetOrdinal("E_ID")),
                E_Date = DateOnly.FromDateTime(date),
                E_participants = reader.GetInt32(reader.GetOrdinal("E_participants")),
                E_Location = reader.GetString(reader.GetOrdinal("E_Location")),
                A_ID = reader.GetInt32(reader.GetOrdinal("A_ID")),
                O_ID = reader.GetInt32(reader.GetOrdinal("O_ID")),
            });
        }

        return results;
    }

    public async Task<int> CreateEventAsync(EventRecord ev, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
INSERT INTO Events (E_Date, E_participants, E_Location, A_ID, O_ID)
OUTPUT INSERTED.E_ID
VALUES (@Date, @Participants, @Location, @AId, @OId);";

        cmd.Parameters.AddWithValue("@Date", ev.E_Date.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@Participants", ev.E_participants);
        cmd.Parameters.AddWithValue("@Location", ev.E_Location);
        cmd.Parameters.AddWithValue("@AId", ev.A_ID);
        cmd.Parameters.AddWithValue("@OId", ev.O_ID);

        var newIdObj = await cmd.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(newIdObj);
    }
}
