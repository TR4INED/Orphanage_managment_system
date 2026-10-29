using Microsoft.AspNetCore.Mvc.RazorPages;
using OrphanageWeb.Data;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Pages.Items;

public class IndexModel : PageModel
{
    private readonly ItemRepository _repository;

    public IndexModel(ItemRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyList<DonationItem> Items { get; private set; } = Array.Empty<DonationItem>();

    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            Items = await _repository.GetItemsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            Items = Array.Empty<DonationItem>();
        }
    }
}
