using Microsoft.Data.SqlClient;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Data;

public sealed class AccountRepository
{
    private readonly string _connectionString;

    public AccountRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("OrphanageDb")
            ?? throw new InvalidOperationException("Missing connection string: ConnectionStrings:OrphanageDb");
    }

    public async Task<string?> ValidateAdminAsync(int adminId, string password, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT A_name
FROM Admin
WHERE A_ID = @Id AND apass = @Pass;";

        cmd.Parameters.AddWithValue("@Id", adminId);
        cmd.Parameters.AddWithValue("@Pass", password);

        var nameObj = await cmd.ExecuteScalarAsync(cancellationToken);
        return nameObj == null || nameObj == DBNull.Value ? null : Convert.ToString(nameObj);
    }

    public async Task<string?> ValidateSponsorAsync(int sponsorId, string password, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT (S_Fname + ' ' + S_Lname) AS SponsorName
FROM Sponsor
WHERE S_ID = @Id AND spass = @Pass;";

        cmd.Parameters.AddWithValue("@Id", sponsorId);
        cmd.Parameters.AddWithValue("@Pass", password);

        var nameObj = await cmd.ExecuteScalarAsync(cancellationToken);
        return nameObj == null || nameObj == DBNull.Value ? null : Convert.ToString(nameObj);
    }

    public async Task<bool> ChangeAdminPasswordAsync(int adminId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
UPDATE Admin
SET apass = @NewPass
WHERE A_ID = @Id AND apass = @CurrentPass;";

        cmd.Parameters.AddWithValue("@Id", adminId);
        cmd.Parameters.AddWithValue("@CurrentPass", currentPassword);
        cmd.Parameters.AddWithValue("@NewPass", newPassword);

        var rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
    }

    public async Task<bool> ChangeSponsorPasswordAsync(int sponsorId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
UPDATE Sponsor
SET spass = @NewPass
WHERE S_ID = @Id AND spass = @CurrentPass;";

        cmd.Parameters.AddWithValue("@Id", sponsorId);
        cmd.Parameters.AddWithValue("@CurrentPass", currentPassword);
        cmd.Parameters.AddWithValue("@NewPass", newPassword);

        var rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
    }

    public async Task<decimal> GetSponsorTotalDonationAsync(int sponsorId, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT total_donation
FROM Sponsor
WHERE S_ID = @Id;";

        cmd.Parameters.AddWithValue("@Id", sponsorId);

        var totalObj = await cmd.ExecuteScalarAsync(cancellationToken);
        if (totalObj == null || totalObj == DBNull.Value)
        {
            return 0m;
        }

        return Convert.ToDecimal(totalObj);
    }

    public async Task<IReadOnlyList<LookupItem>> GetSponsoredOrphansAsync(int sponsorId, CancellationToken cancellationToken = default)
    {
        var results = new List<LookupItem>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT o.O_ID AS Id, (o.O_Fname + ' ' + o.O_Lname) AS Name
FROM Sponsored_by sb
INNER JOIN Orphan o ON o.O_ID = sb.O_ID
WHERE sb.S_ID = @SId
ORDER BY o.O_ID;";

        cmd.Parameters.AddWithValue("@SId", sponsorId);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new LookupItem
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
            });
        }

        return results;
    }

    public async Task IncreaseSponsorDonationAsync(int sponsorId, int orphanId, decimal amount, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var tx = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            await using (var cmdCheck = connection.CreateCommand())
            {
                cmdCheck.Transaction = (SqlTransaction)tx;
                cmdCheck.CommandText = @"
SELECT COUNT(1)
FROM Sponsored_by
WHERE S_ID = @SId AND O_ID = @OId;";

                cmdCheck.Parameters.AddWithValue("@SId", sponsorId);
                cmdCheck.Parameters.AddWithValue("@OId", orphanId);

                var existsObj = await cmdCheck.ExecuteScalarAsync(cancellationToken);
                var exists = Convert.ToInt32(existsObj) > 0;

                if (!exists)
                {
                    throw new InvalidOperationException("This sponsor is not mapped to the selected orphan.");
                }
            }

            await using (var cmdUpdate = connection.CreateCommand())
            {
                cmdUpdate.Transaction = (SqlTransaction)tx;
                cmdUpdate.CommandText = @"
UPDATE Sponsor
SET total_donation = ISNULL(total_donation, 0) + @Amount
WHERE S_ID = @SId;";

                cmdUpdate.Parameters.AddWithValue("@SId", sponsorId);
                cmdUpdate.Parameters.AddWithValue("@Amount", amount);

                await cmdUpdate.ExecuteNonQueryAsync(cancellationToken);
            }

            await using (var cmdDonation = connection.CreateCommand())
            {
                cmdDonation.Transaction = (SqlTransaction)tx;
                cmdDonation.CommandText = @"
INSERT INTO Donations (D_date, S_ID, O_ID, A_ID)
OUTPUT INSERTED.D_ID
VALUES (@Date, @SId, @OId, 1);";

                cmdDonation.Parameters.AddWithValue("@Date", DateTime.Today);
                cmdDonation.Parameters.AddWithValue("@SId", sponsorId);
                cmdDonation.Parameters.AddWithValue("@OId", orphanId);

                var newDonationIdObj = await cmdDonation.ExecuteScalarAsync(cancellationToken);
                var newDonationId = Convert.ToInt32(newDonationIdObj);

                await using var cmdItem = connection.CreateCommand();
                cmdItem.Transaction = (SqlTransaction)tx;
                cmdItem.CommandText = @"
INSERT INTO Item (I_type, I_name, I_quantity, D_ID)
VALUES (@Type, @Name, @Qty, @DId);";
                cmdItem.Parameters.AddWithValue("@Type", "Money");
                cmdItem.Parameters.AddWithValue("@Name", "Sponsor Donation");
                cmdItem.Parameters.AddWithValue("@Qty", 1);
                cmdItem.Parameters.AddWithValue("@DId", newDonationId);
                await cmdItem.ExecuteNonQueryAsync(cancellationToken);
            }

            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
