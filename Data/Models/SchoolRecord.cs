namespace OrphanageWeb.Data.Models;

public sealed class SchoolRecord
{
    public int Sch_ID { get; set; }
    public string Sch_name { get; set; } = string.Empty;
    public string Sch_address { get; set; } = string.Empty;
    public string Sch_email { get; set; } = string.Empty;
    public int EducationRecordCount { get; set; }
}
