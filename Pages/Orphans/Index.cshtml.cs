using Microsoft.AspNetCore.Mvc.RazorPages;
using OrphanageWeb.Data;
using OrphanageWeb.Data.Models;
using System.Threading;

namespace OrphanageWeb.Pages.Orphans;

public class IndexModel : PageModel
{
    private readonly OrphanRepository _repository;

    public IndexModel(OrphanRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyList<Orphan> Orphans { get; private set; } = Array.Empty<Orphan>();

    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            Orphans = await _repository.GetOrphansAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            Orphans = Array.Empty<Orphan>();
        }
    }
}
