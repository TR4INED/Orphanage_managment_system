namespace OrphanageWeb.Data.Models;

public sealed class EventRecord
{
    public int E_ID { get; set; }
    public DateOnly E_Date { get; set; }
    public int E_participants { get; set; }
    public string E_Location { get; set; } = string.Empty;
    public int A_ID { get; set; }
    public int O_ID { get; set; }
}
