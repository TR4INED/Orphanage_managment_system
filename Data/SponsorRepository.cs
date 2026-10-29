using Microsoft.Data.SqlClient;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Data;

public sealed class SponsorRepository
{
    private readonly string _connectionString;

    public SponsorRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("OrphanageDb")
            ?? throw new InvalidOperationException("Missing connection string: ConnectionStrings:OrphanageDb");
    }

    public async Task<IReadOnlyList<Sponsor>> GetSponsorsAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<Sponsor>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT s.S_ID, s.S_address, s.S_Fname, s.S_Lname, s.S_email, s.total_donation, COUNT(sb.O_ID) AS SponsoredCount
FROM Sponsor s
LEFT JOIN Sponsored_by sb ON sb.S_ID = s.S_ID
GROUP BY s.S_ID, s.S_address, s.S_Fname, s.S_Lname, s.S_email, s.total_donation
ORDER BY s.S_ID;";

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new Sponsor
            {
                S_ID = reader.GetInt32(reader.GetOrdinal("S_ID")),
                S_address = reader.GetString(reader.GetOrdinal("S_address")),
                S_Fname = reader.GetString(reader.GetOrdinal("S_Fname")),
                S_Lname = reader.GetString(reader.GetOrdinal("S_Lname")),
                S_email = reader.GetString(reader.GetOrdinal("S_email")),
                total_donation = reader.GetDecimal(reader.GetOrdinal("total_donation")),
                SponsoredCount = reader.GetInt32(reader.GetOrdinal("SponsoredCount")),
            });
        }

        return results;
    }

    public async Task<int> CreateSponsorAsync(Sponsor sponsor, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var tx = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            await using var cmd = connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tx;
            cmd.CommandText = @"
INSERT INTO Sponsor (S_address, S_Fname, S_Lname, S_email, total_donation)
OUTPUT INSERTED.S_ID
VALUES (@Address, @Fname, @Lname, @Email, @TotalDonation);";

            cmd.Parameters.AddWithValue("@Address", sponsor.S_address);
            cmd.Parameters.AddWithValue("@Fname", sponsor.S_Fname);
            cmd.Parameters.AddWithValue("@Lname", sponsor.S_Lname);
            cmd.Parameters.AddWithValue("@Email", sponsor.S_email);
            cmd.Parameters.AddWithValue("@TotalDonation", sponsor.total_donation);

            var newIdObj = await cmd.ExecuteScalarAsync(cancellationToken);
            var newId = Convert.ToInt32(newIdObj);

            await using var cmdPass = connection.CreateCommand();
            cmdPass.Transaction = (SqlTransaction)tx;
            cmdPass.CommandText = @"
UPDATE Sponsor
SET spass = CONCAT('s', CAST(@Id AS varchar(12)))
WHERE S_ID = @Id AND (spass IS NULL OR LTRIM(RTRIM(spass)) = '');";
            cmdPass.Parameters.AddWithValue("@Id", newId);
            await cmdPass.ExecuteNonQueryAsync(cancellationToken);

            await tx.CommitAsync(cancellationToken);
            return newId;
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
