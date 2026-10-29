using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrphanageWeb.Data;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Pages.EducationRecords;

public class CreateModel : PageModel
{
    private readonly EducationRecordRepository _repository;

    public CreateModel(EducationRecordRepository repository)
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
            var record = new EducationRecord
            {
                Date_acquired = Input.DateAcquired,
                certificate = Input.Certificate,
                O_ID = Input.OrphanId,
                Sch_ID = Input.SchoolId,
            };

            await _repository.CreateEducationRecordAsync(record, cancellationToken);
            return RedirectToPage("/EducationRecords/Index");
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
        public DateOnly DateAcquired { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Required]
        [StringLength(200)]
        public string Certificate { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int OrphanId { get; set; } = 1;

        [Range(1, int.MaxValue)]
        public int SchoolId { get; set; } = 1;
    }
}
