using Microsoft.AspNetCore.Mvc.RazorPages;
using OrphanageWeb.Data;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Pages.MealPlans;

public class IndexModel : PageModel
{
    private readonly MealplanRepository _repository;

    public IndexModel(MealplanRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyList<MealplanRecord> Mealplans { get; private set; } = Array.Empty<MealplanRecord>();

    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            Mealplans = await _repository.GetMealplansAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            Mealplans = Array.Empty<MealplanRecord>();
        }
    }
}
