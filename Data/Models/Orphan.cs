namespace OrphanageWeb.Data.Models;

public sealed class Orphan
{
    public int O_ID { get; set; }
    public string O_Fname { get; set; } = string.Empty;
    public string O_Lname { get; set; } = string.Empty;
    public string O_gender { get; set; } = string.Empty;
    public DateOnly O_DoB { get; set; }
    public int A_ID { get; set; }
}
