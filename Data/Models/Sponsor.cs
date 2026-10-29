namespace OrphanageWeb.Data.Models;

public sealed class Sponsor
{
    public int S_ID { get; set; }
    public string S_address { get; set; } = string.Empty;
    public string S_Fname { get; set; } = string.Empty;
    public string S_Lname { get; set; } = string.Empty;
    public string S_email { get; set; } = string.Empty;
    public decimal total_donation { get; set; }
    public int SponsoredCount { get; set; }
}
