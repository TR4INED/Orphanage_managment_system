using Microsoft.Data.SqlClient;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Data;

public sealed class SchoolRepository
{
    private readonly string _connectionString;

    public SchoolRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("OrphanageDb")
            ?? throw new InvalidOperationException("Missing connection string: ConnectionStrings:OrphanageDb");
    }

    public async Task<IReadOnlyList<SchoolRecord>> GetSchoolsAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<SchoolRecord>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT s.Sch_ID, s.Sch_name, s.Sch_address, s.Sch_email, COUNT(er.R_ID) AS EducationRecordCount
FROM School s
LEFT JOIN Education_record er ON er.Sch_ID = s.Sch_ID
GROUP BY s.Sch_ID, s.Sch_name, s.Sch_address, s.Sch_email
ORDER BY s.Sch_ID;";

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new SchoolRecord
            {
                Sch_ID = reader.GetInt32(reader.GetOrdinal("Sch_ID")),
                Sch_name = reader.GetString(reader.GetOrdinal("Sch_name")),
                Sch_address = reader.GetString(reader.GetOrdinal("Sch_address")),
                Sch_email = reader.GetString(reader.GetOrdinal("Sch_email")),
                EducationRecordCount = reader.GetInt32(reader.GetOrdinal("EducationRecordCount")),
            });
        }

        return results;
    }

    public async Task<int> CreateSchoolAsync(SchoolRecord school, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
INSERT INTO School (Sch_name, Sch_address, Sch_email)
OUTPUT INSERTED.Sch_ID
VALUES (@Name, @Address, @Email);";

        cmd.Parameters.AddWithValue("@Name", school.Sch_name);
        cmd.Parameters.AddWithValue("@Address", school.Sch_address);
        cmd.Parameters.AddWithValue("@Email", school.Sch_email);

        var newIdObj = await cmd.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(newIdObj);
    }
}
