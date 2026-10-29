using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrphanageWeb.Data;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Pages.Sponsors;

public class CreateModel : PageModel
{
    private readonly SponsorRepository _repository;

    public CreateModel(SponsorRepository repository)
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
            var sponsor = new Sponsor
            {
                S_Fname = Input.FirstName,
                S_Lname = Input.LastName,
                S_email = Input.Email,
                S_address = Input.Address,
                total_donation = Input.TotalDonation,
            };

            await _repository.CreateSponsorAsync(sponsor, cancellationToken);
            return RedirectToPage("/Sponsors/Index");
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
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Range(0, 999999999)]
        public decimal TotalDonation { get; set; }
    }
}
