using Microsoft.AspNetCore.Mvc.RazorPages;
using OrphanageWeb.Data;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Pages.Sponsors;

public class IndexModel : PageModel
{
    private readonly SponsorRepository _repository;

    public IndexModel(SponsorRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyList<Sponsor> Sponsors { get; private set; } = Array.Empty<Sponsor>();

    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            Sponsors = await _repository.GetSponsorsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            Sponsors = Array.Empty<Sponsor>();
        }
    }
}
