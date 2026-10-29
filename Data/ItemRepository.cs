using Microsoft.Data.SqlClient;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Data;

public sealed class ItemRepository
{
    private readonly string _connectionString;

    public ItemRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("OrphanageDb")
            ?? throw new InvalidOperationException("Missing connection string: ConnectionStrings:OrphanageDb");
    }

    public async Task<IReadOnlyList<DonationItem>> GetItemsAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<DonationItem>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT I_ID, I_type, I_name, I_quantity, D_ID
FROM Item
ORDER BY I_ID;";

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new DonationItem
            {
                I_ID = reader.GetInt32(reader.GetOrdinal("I_ID")),
                I_type = reader.GetString(reader.GetOrdinal("I_type")),
                I_name = reader.GetString(reader.GetOrdinal("I_name")),
                I_quantity = reader.GetInt32(reader.GetOrdinal("I_quantity")),
                D_ID = reader.GetInt32(reader.GetOrdinal("D_ID")),
            });
        }

        return results;
    }
}
