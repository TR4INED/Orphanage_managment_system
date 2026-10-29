using Microsoft.AspNetCore.Mvc.RazorPages;
using OrphanageWeb.Data;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Pages.Volunteers;

public class IndexModel : PageModel
{
    private readonly VolunteerRepository _repository;

    public IndexModel(VolunteerRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyList<Volunteer> Volunteers { get; private set; } = Array.Empty<Volunteer>();

    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            Volunteers = await _repository.GetVolunteersAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            Volunteers = Array.Empty<Volunteer>();
        }
    }
}
