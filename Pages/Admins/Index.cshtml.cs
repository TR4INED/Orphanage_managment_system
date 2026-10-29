using Microsoft.AspNetCore.Mvc.RazorPages;
using OrphanageWeb.Data;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Pages.Admins;

public class IndexModel : PageModel
{
    private readonly AdminRepository _repository;

    public IndexModel(AdminRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyList<AdminRecord> Admins { get; private set; } = Array.Empty<AdminRecord>();

    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            Admins = await _repository.GetAdminsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            Admins = Array.Empty<AdminRecord>();
        }
    }
}
