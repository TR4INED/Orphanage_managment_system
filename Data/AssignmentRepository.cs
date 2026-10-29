using Microsoft.Data.SqlClient;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Data;

public sealed class AssignmentRepository
{
    private readonly string _connectionString;

    public AssignmentRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("OrphanageDb")
            ?? throw new InvalidOperationException("Missing connection string: ConnectionStrings:OrphanageDb");
    }

    public async Task<IReadOnlyList<AssistedByMapping>> GetAssistedByAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<AssistedByMapping>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT ab.V_ID, ab.O_ID,
       v.V_name AS VolunteerName,
       CONCAT(o.O_Fname, ' ', o.O_Lname) AS OrphanName
FROM Assisted_by ab
LEFT JOIN Volunteer v ON v.V_ID = ab.V_ID
LEFT JOIN Orphan o ON o.O_ID = ab.O_ID
ORDER BY ab.V_ID, ab.O_ID;";

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new AssistedByMapping
            {
                V_ID = reader.GetInt32(reader.GetOrdinal("V_ID")),
                O_ID = reader.GetInt32(reader.GetOrdinal("O_ID")),
                VolunteerName = reader.IsDBNull(reader.GetOrdinal("VolunteerName")) ? null : reader.GetString(reader.GetOrdinal("VolunteerName")),
                OrphanName = reader.IsDBNull(reader.GetOrdinal("OrphanName")) ? null : reader.GetString(reader.GetOrdinal("OrphanName")),
            });
        }

        return results;
    }

    public async Task<IReadOnlyList<LookupItem>> GetVolunteerOptionsAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<LookupItem>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT V_ID, V_name
FROM Volunteer
ORDER BY V_name, V_ID;";

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new LookupItem
            {
                Id = reader.GetInt32(reader.GetOrdinal("V_ID")),
                Name = reader.GetString(reader.GetOrdinal("V_name")),
            });
        }

        return results;
    }

    public async Task<IReadOnlyList<LookupItem>> GetSponsorOptionsAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<LookupItem>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT S_ID, CONCAT(S_Fname, ' ', S_Lname) AS SponsorName
FROM Sponsor
ORDER BY SponsorName, S_ID;";

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new LookupItem
            {
                Id = reader.GetInt32(reader.GetOrdinal("S_ID")),
                Name = reader.GetString(reader.GetOrdinal("SponsorName")),
            });
        }

        return results;
    }

    public async Task<IReadOnlyList<LookupItem>> GetMealplanOptionsAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<LookupItem>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT MP_ID, MP_meal
FROM Mealplan
ORDER BY MP_ID;";

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new LookupItem
            {
                Id = reader.GetInt32(reader.GetOrdinal("MP_ID")),
                Name = reader.GetString(reader.GetOrdinal("MP_meal")),
            });
        }

        return results;
    }

    public async Task<IReadOnlyList<LookupItem>> GetOrphanOptionsAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<LookupItem>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT O_ID, CONCAT(O_Fname, ' ', O_Lname) AS OrphanName
FROM Orphan
ORDER BY OrphanName, O_ID;";

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new LookupItem
            {
                Id = reader.GetInt32(reader.GetOrdinal("O_ID")),
                Name = reader.GetString(reader.GetOrdinal("OrphanName")),
            });
        }

        return results;
    }

    public async Task<IReadOnlyList<SponsoredByMapping>> GetSponsoredByAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<SponsoredByMapping>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT sb.S_ID, sb.O_ID,
       CONCAT(s.S_Fname, ' ', s.S_Lname) AS SponsorName,
       CONCAT(o.O_Fname, ' ', o.O_Lname) AS OrphanName
FROM Sponsored_by sb
LEFT JOIN Sponsor s ON s.S_ID = sb.S_ID
LEFT JOIN Orphan o ON o.O_ID = sb.O_ID
ORDER BY sb.S_ID, sb.O_ID;";

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new SponsoredByMapping
            {
                S_ID = reader.GetInt32(reader.GetOrdinal("S_ID")),
                O_ID = reader.GetInt32(reader.GetOrdinal("O_ID")),
                SponsorName = reader.IsDBNull(reader.GetOrdinal("SponsorName")) ? null : reader.GetString(reader.GetOrdinal("SponsorName")),
                OrphanName = reader.IsDBNull(reader.GetOrdinal("OrphanName")) ? null : reader.GetString(reader.GetOrdinal("OrphanName")),
            });
        }

        return results;
    }

    public async Task<IReadOnlyList<ServedToMapping>> GetServedToAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<ServedToMapping>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT st.MP_ID, st.O_ID,
       mp.MP_meal AS MealplanName,
       CONCAT(o.O_Fname, ' ', o.O_Lname) AS OrphanName
FROM Served_to st
LEFT JOIN Mealplan mp ON mp.MP_ID = st.MP_ID
LEFT JOIN Orphan o ON o.O_ID = st.O_ID
ORDER BY st.MP_ID, st.O_ID;";

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new ServedToMapping
            {
                MP_ID = reader.GetInt32(reader.GetOrdinal("MP_ID")),
                O_ID = reader.GetInt32(reader.GetOrdinal("O_ID")),
                MealplanName = reader.IsDBNull(reader.GetOrdinal("MealplanName")) ? null : reader.GetString(reader.GetOrdinal("MealplanName")),
                OrphanName = reader.IsDBNull(reader.GetOrdinal("OrphanName")) ? null : reader.GetString(reader.GetOrdinal("OrphanName")),
            });
        }

        return results;
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

    public async Task CreateSponsoredByAsync(int sponsorId, int orphanId, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
INSERT INTO Sponsored_by (S_ID, O_ID)
VALUES (@SId, @OId);";

        cmd.Parameters.AddWithValue("@SId", sponsorId);
        cmd.Parameters.AddWithValue("@OId", orphanId);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task CreateServedToAsync(int mealplanId, int orphanId, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
INSERT INTO Served_to (MP_ID, O_ID)
VALUES (@MPId, @OId);";

        cmd.Parameters.AddWithValue("@MPId", mealplanId);
        cmd.Parameters.AddWithValue("@OId", orphanId);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }
}
