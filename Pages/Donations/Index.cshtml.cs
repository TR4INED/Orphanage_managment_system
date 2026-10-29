using Microsoft.AspNetCore.Mvc.RazorPages;
using OrphanageWeb.Data;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Pages.Donations;

public class IndexModel : PageModel
{
    private readonly DonationRepository _repository;

    public IndexModel(DonationRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyList<DonationRecord> Donations { get; private set; } = Array.Empty<DonationRecord>();

    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            Donations = await _repository.GetDonationsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            Donations = Array.Empty<DonationRecord>();
        }
    }
}
