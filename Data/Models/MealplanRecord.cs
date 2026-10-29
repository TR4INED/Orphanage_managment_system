namespace OrphanageWeb.Data.Models;

public sealed class MealplanRecord
{
    public int MP_ID { get; set; }
    public string MP_meal { get; set; } = string.Empty;
    public int O_ID { get; set; }
    public int ServedCount { get; set; }
}
