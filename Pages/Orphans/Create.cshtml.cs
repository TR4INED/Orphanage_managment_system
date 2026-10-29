using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using OrphanageWeb.Data;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Pages.Orphans;

public class CreateModel : PageModel
{
    private readonly OrphanRepository _repository;

    public CreateModel(OrphanRepository repository)
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
            var orphan = new Orphan
            {
                O_Fname = Input.FirstName,
                O_Lname = Input.LastName,
                O_gender = Input.Gender,
                O_DoB = Input.DateOfBirth,
                A_ID = Input.AdminId,
            };

            await _repository.CreateOrphanAsync(orphan, cancellationToken);
            return RedirectToPage("/Orphans/Index");
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
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Gender { get; set; } = "Male";

        [Required]
        public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Range(1, int.MaxValue)]
        public int AdminId { get; set; } = 1;
    }
}
