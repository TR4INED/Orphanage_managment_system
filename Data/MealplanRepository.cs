using Microsoft.Data.SqlClient;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Data;

public sealed class MealplanRepository
{
    private readonly string _connectionString;

    public MealplanRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("OrphanageDb")
            ?? throw new InvalidOperationException("Missing connection string: ConnectionStrings:OrphanageDb");
    }

    public async Task<IReadOnlyList<MealplanRecord>> GetMealplansAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<MealplanRecord>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT mp.MP_ID, mp.MP_meal, mp.O_ID, COUNT(st.O_ID) AS ServedCount
FROM Mealplan mp
LEFT JOIN Served_to st ON st.MP_ID = mp.MP_ID
GROUP BY mp.MP_ID, mp.MP_meal, mp.O_ID
ORDER BY mp.MP_ID;";

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new MealplanRecord
            {
                MP_ID = reader.GetInt32(reader.GetOrdinal("MP_ID")),
                MP_meal = reader.GetString(reader.GetOrdinal("MP_meal")),
                O_ID = reader.GetInt32(reader.GetOrdinal("O_ID")),
                ServedCount = reader.GetInt32(reader.GetOrdinal("ServedCount")),
            });
        }

        return results;
    }

    public async Task<int> CreateMealplanAndServeAsync(MealplanRecord mealplan, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var tx = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            await using var cmdPlan = connection.CreateCommand();
            cmdPlan.Transaction = (SqlTransaction)tx;
            cmdPlan.CommandText = @"
INSERT INTO Mealplan (MP_meal, O_ID)
OUTPUT INSERTED.MP_ID
VALUES (@Meal, @OId);";

            cmdPlan.Parameters.AddWithValue("@Meal", mealplan.MP_meal);
            cmdPlan.Parameters.AddWithValue("@OId", mealplan.O_ID);

            var newIdObj = await cmdPlan.ExecuteScalarAsync(cancellationToken);
            var newId = Convert.ToInt32(newIdObj);

            await using var cmdServed = connection.CreateCommand();
            cmdServed.Transaction = (SqlTransaction)tx;
            cmdServed.CommandText = @"
INSERT INTO Served_to (MP_ID, O_ID)
VALUES (@MPId, @OId);";

            cmdServed.Parameters.AddWithValue("@MPId", newId);
            cmdServed.Parameters.AddWithValue("@OId", mealplan.O_ID);

            await cmdServed.ExecuteNonQueryAsync(cancellationToken);

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
