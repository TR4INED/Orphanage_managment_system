using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrphanageWeb.Data;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Pages.Donations;

public class CreateModel : PageModel
{
    private readonly DonationRepository _repository;

    public CreateModel(DonationRepository repository)
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
            var donation = new DonationRecord
            {
                D_date = Input.DonationDate,
                S_ID = Input.SponsorId,
                O_ID = Input.OrphanId,
                A_ID = Input.AdminId,
            };

            var item = new DonationItem
            {
                I_type = Input.ItemType,
                I_name = Input.ItemName,
                I_quantity = Input.Quantity,
            };

            await _repository.CreateDonationWithItemAsync(donation, item, cancellationToken);
            return RedirectToPage("/Donations/Index");
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
        public DateOnly DonationDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Range(1, int.MaxValue)]
        public int SponsorId { get; set; } = 1;

        [Range(1, int.MaxValue)]
        public int OrphanId { get; set; } = 1;

        [Range(1, int.MaxValue)]
        public int AdminId { get; set; } = 1;

        [Required]
        [StringLength(100)]
        public string ItemType { get; set; } = "Clothes";

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;
    }
}
