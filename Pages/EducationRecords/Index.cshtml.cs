using Microsoft.AspNetCore.Mvc.RazorPages;
using OrphanageWeb.Data;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Pages.EducationRecords;

public class IndexModel : PageModel
{
    private readonly EducationRecordRepository _repository;

    public IndexModel(EducationRecordRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyList<EducationRecord> Records { get; private set; } = Array.Empty<EducationRecord>();

    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            Records = await _repository.GetEducationRecordsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            Records = Array.Empty<EducationRecord>();
        }
    }
}
