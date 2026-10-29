using Microsoft.Data.SqlClient;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Data;

public sealed class OrphanRepository
{
    private readonly string _connectionString;

    public OrphanRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("OrphanageDb")
            ?? throw new InvalidOperationException("Missing connection string: ConnectionStrings:OrphanageDb");
    }

    public async Task<IReadOnlyList<Orphan>> GetOrphansAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<Orphan>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT O_ID, O_Fname, O_Lname, O_gender, O_DoB, A_ID
FROM Orphan
ORDER BY O_ID;";

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var dob = reader.GetDateTime(reader.GetOrdinal("O_DoB"));

            results.Add(new Orphan
            {
                O_ID = reader.GetInt32(reader.GetOrdinal("O_ID")),
                O_Fname = reader.GetString(reader.GetOrdinal("O_Fname")),
                O_Lname = reader.GetString(reader.GetOrdinal("O_Lname")),
                O_gender = reader.GetString(reader.GetOrdinal("O_gender")),
                O_DoB = DateOnly.FromDateTime(dob),
                A_ID = reader.GetInt32(reader.GetOrdinal("A_ID")),
            });
        }

        return results;
    }

    public async Task<int> CreateOrphanAsync(Orphan orphan, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
INSERT INTO Orphan (O_Fname, O_Lname, O_gender, O_DoB, A_ID)
OUTPUT INSERTED.O_ID
VALUES (@Fname, @Lname, @Gender, @DoB, @AId);";

        cmd.Parameters.AddWithValue("@Fname", orphan.O_Fname);
        cmd.Parameters.AddWithValue("@Lname", orphan.O_Lname);
        cmd.Parameters.AddWithValue("@Gender", orphan.O_gender);
        cmd.Parameters.AddWithValue("@DoB", orphan.O_DoB.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@AId", orphan.A_ID);

        var newIdObj = await cmd.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(newIdObj);
    }
}
