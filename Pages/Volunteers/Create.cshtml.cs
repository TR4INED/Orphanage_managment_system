using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrphanageWeb.Data;

namespace OrphanageWeb.Pages.Volunteers;

public class CreateModel : PageModel
{
    private readonly VolunteerRepository _repository;

    public CreateModel(VolunteerRepository repository)
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
            var newId = await _repository.CreateVolunteerAsync(Input.Name, Input.Hours, cancellationToken);

            if (Input.AssistOrphanId is not null)
            {
                await _repository.CreateAssistedByAsync(newId, Input.AssistOrphanId.Value, cancellationToken);
            }

            return RedirectToPage("/Volunteers/Index");
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
        public string Name { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Hours { get; set; }

        [Range(1, int.MaxValue)]
        public int? AssistOrphanId { get; set; }
    }
}
