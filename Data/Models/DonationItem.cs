namespace OrphanageWeb.Data.Models;

public sealed class DonationItem
{
    public int I_ID { get; set; }
    public string I_type { get; set; } = string.Empty;
    public string I_name { get; set; } = string.Empty;
    public int I_quantity { get; set; }
    public int D_ID { get; set; }
}
