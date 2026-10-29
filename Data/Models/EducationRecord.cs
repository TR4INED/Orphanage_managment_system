namespace OrphanageWeb.Data.Models;

public sealed class EducationRecord
{
    public int R_ID { get; set; }
    public DateOnly Date_acquired { get; set; }
    public string certificate { get; set; } = string.Empty;
    public int O_ID { get; set; }
    public int Sch_ID { get; set; }

    public string? OrphanName { get; set; }
    public string? SchoolName { get; set; }
}
