using Microsoft.AspNetCore.Mvc.RazorPages;
using OrphanageWeb.Data;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Pages.Events;

public class IndexModel : PageModel
{
    private readonly EventRepository _repository;

    public IndexModel(EventRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyList<EventRecord> Events { get; private set; } = Array.Empty<EventRecord>();

    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            Events = await _repository.GetEventsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            Events = Array.Empty<EventRecord>();
        }
    }
}
