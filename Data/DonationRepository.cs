using Microsoft.Data.SqlClient;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Data;

public sealed class DonationRepository
{
    private readonly string _connectionString;

    public DonationRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("OrphanageDb")
            ?? throw new InvalidOperationException("Missing connection string: ConnectionStrings:OrphanageDb");
    }

    public async Task<IReadOnlyList<DonationRecord>> GetDonationsAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<DonationRecord>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT d.D_ID, d.D_date, d.S_ID, d.O_ID, d.A_ID, COUNT(i.I_ID) AS ItemCount
FROM Donations d
LEFT JOIN Item i ON i.D_ID = d.D_ID
GROUP BY d.D_ID, d.D_date, d.S_ID, d.O_ID, d.A_ID
ORDER BY d.D_ID;";

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var date = reader.GetDateTime(reader.GetOrdinal("D_date"));

            int? adminId = null;
            var adminOrdinal = reader.GetOrdinal("A_ID");
            if (!reader.IsDBNull(adminOrdinal))
            {
                adminId = reader.GetInt32(adminOrdinal);
            }

            results.Add(new DonationRecord
            {
                D_ID = reader.GetInt32(reader.GetOrdinal("D_ID")),
                D_date = DateOnly.FromDateTime(date),
                S_ID = reader.GetInt32(reader.GetOrdinal("S_ID")),
                O_ID = reader.GetInt32(reader.GetOrdinal("O_ID")),
                A_ID = adminId,
                ItemCount = reader.GetInt32(reader.GetOrdinal("ItemCount")),
            });
        }

        return results;
    }

    public async Task<int> CreateDonationWithItemAsync(DonationRecord donation, DonationItem item, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var tx = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            await using var cmdDonation = connection.CreateCommand();
            cmdDonation.Transaction = (SqlTransaction)tx;
            cmdDonation.CommandText = @"
INSERT INTO Donations (D_date, S_ID, O_ID, A_ID)
OUTPUT INSERTED.D_ID
VALUES (@Date, @SId, @OId, @AId);";

            cmdDonation.Parameters.AddWithValue("@Date", donation.D_date.ToDateTime(TimeOnly.MinValue));
            cmdDonation.Parameters.AddWithValue("@SId", donation.S_ID);
            cmdDonation.Parameters.AddWithValue("@OId", donation.O_ID);
            cmdDonation.Parameters.AddWithValue("@AId", donation.A_ID);

            var newDonationIdObj = await cmdDonation.ExecuteScalarAsync(cancellationToken);
            var newDonationId = Convert.ToInt32(newDonationIdObj);

            await using var cmdItem = connection.CreateCommand();
            cmdItem.Transaction = (SqlTransaction)tx;
            cmdItem.CommandText = @"
INSERT INTO Item (I_type, I_name, I_quantity, D_ID)
VALUES (@Type, @Name, @Qty, @DId);";

            cmdItem.Parameters.AddWithValue("@Type", item.I_type);
            cmdItem.Parameters.AddWithValue("@Name", item.I_name);
            cmdItem.Parameters.AddWithValue("@Qty", item.I_quantity);
            cmdItem.Parameters.AddWithValue("@DId", newDonationId);

            await cmdItem.ExecuteNonQueryAsync(cancellationToken);

            await tx.CommitAsync(cancellationToken);
            return newDonationId;
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
