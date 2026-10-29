namespace OrphanageWeb.Data.Models;

public sealed class Volunteer
{
    public int V_ID { get; set; }
    public string V_name { get; set; } = string.Empty;
    public int V_hours { get; set; }
    public int AssistedCount { get; set; }
}
