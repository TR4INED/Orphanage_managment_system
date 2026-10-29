using Microsoft.AspNetCore.Mvc.RazorPages;
using OrphanageWeb.Data;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Pages.Schools;

public class IndexModel : PageModel
{
    private readonly SchoolRepository _repository;

    public IndexModel(SchoolRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyList<SchoolRecord> Schools { get; private set; } = Array.Empty<SchoolRecord>();

    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            Schools = await _repository.GetSchoolsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            Schools = Array.Empty<SchoolRecord>();
        }
    }
}
