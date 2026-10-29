using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrphanageWeb.Data;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Pages.MealPlans;

public class CreateModel : PageModel
{
    private readonly MealplanRepository _repository;

    public CreateModel(MealplanRepository repository)
    {
        _repository = repository;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; private set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var mealplan = new MealplanRecord
            {
                MP_meal = Input.Meal,
                O_ID = Input.OrphanId,
            };

            await _repository.CreateMealplanAndServeAsync(mealplan, cancellationToken);
            return RedirectToPage("/MealPlans/Index");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }

    public sealed class InputModel
    {
        [Required]
        [StringLength(100)]
        public string Meal { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int OrphanId { get; set; } = 1;
    }
}
