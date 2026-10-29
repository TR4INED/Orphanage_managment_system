using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrphanageWeb.Data;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Pages.Events;

public class CreateModel : PageModel
{
    private readonly EventRepository _repository;

    public CreateModel(EventRepository repository)
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
            var ev = new EventRecord
            {
                E_Date = Input.EventDate,
                E_participants = Input.Participants,
                E_Location = Input.Location,
                A_ID = Input.AdminId,
                O_ID = Input.OrphanId,
            };

            await _repository.CreateEventAsync(ev, cancellationToken);
            return RedirectToPage("/Events/Index");
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
        public DateOnly EventDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Range(0, int.MaxValue)]
        public int Participants { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int AdminId { get; set; } = 1;

        [Range(1, int.MaxValue)]
        public int OrphanId { get; set; } = 1;
    }
}
