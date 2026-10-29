namespace OrphanageWeb.Data.Models;

public sealed class DonationRecord
{
    public int D_ID { get; set; }
    public DateOnly D_date { get; set; }
    public int S_ID { get; set; }
    public int O_ID { get; set; }
    public int? A_ID { get; set; }
    public int ItemCount { get; set; }
}
