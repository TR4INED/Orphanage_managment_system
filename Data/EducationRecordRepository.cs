using Microsoft.Data.SqlClient;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Data;

public sealed class EducationRecordRepository
{
    private readonly string _connectionString;

    public EducationRecordRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("OrphanageDb")
            ?? throw new InvalidOperationException("Missing connection string: ConnectionStrings:OrphanageDb");
    }

    public async Task<IReadOnlyList<EducationRecord>> GetEducationRecordsAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<EducationRecord>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT er.R_ID, er.Date_acquired, er.certificate, er.O_ID, er.Sch_ID,
       CONCAT(o.O_Fname, ' ', o.O_Lname) AS OrphanName,
       s.Sch_name AS SchoolName
FROM Education_record er
LEFT JOIN Orphan o ON o.O_ID = er.O_ID
LEFT JOIN School s ON s.Sch_ID = er.Sch_ID
ORDER BY er.R_ID;";

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var date = reader.GetDateTime(reader.GetOrdinal("Date_acquired"));

            results.Add(new EducationRecord
            {
                R_ID = reader.GetInt32(reader.GetOrdinal("R_ID")),
                Date_acquired = DateOnly.FromDateTime(date),
                certificate = reader.GetString(reader.GetOrdinal("certificate")),
                O_ID = reader.GetInt32(reader.GetOrdinal("O_ID")),
                Sch_ID = reader.GetInt32(reader.GetOrdinal("Sch_ID")),
                OrphanName = reader.IsDBNull(reader.GetOrdinal("OrphanName")) ? null : reader.GetString(reader.GetOrdinal("OrphanName")),
                SchoolName = reader.IsDBNull(reader.GetOrdinal("SchoolName")) ? null : reader.GetString(reader.GetOrdinal("SchoolName")),
            });
        }

        return results;
    }

    public async Task<int> CreateEducationRecordAsync(EducationRecord record, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
INSERT INTO Education_record (Date_acquired, certificate, O_ID, Sch_ID)
OUTPUT INSERTED.R_ID
VALUES (@DateAcquired, @Certificate, @OId, @SchId);";

        cmd.Parameters.AddWithValue("@DateAcquired", record.Date_acquired.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@Certificate", record.certificate);
        cmd.Parameters.AddWithValue("@OId", record.O_ID);
        cmd.Parameters.AddWithValue("@SchId", record.Sch_ID);

        var newIdObj = await cmd.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(newIdObj);
    }
}
