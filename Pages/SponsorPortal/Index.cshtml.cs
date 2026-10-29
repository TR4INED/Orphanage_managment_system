using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrphanageWeb.Data;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Pages.SponsorPortal;

public class IndexModel : PageModel
{
    private readonly AccountRepository _accounts;

    public IndexModel(AccountRepository accounts)
    {
        _accounts = accounts;
    }

    public IReadOnlyList<LookupItem> SponsoredOrphans { get; private set; } = Array.Empty<LookupItem>();

    public decimal TotalDonation { get; private set; }

    public string? ErrorMessage { get; private set; }

    [TempData]
    public string? StatusMessage { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var sponsorId = GetSponsorId();
        if (sponsorId == null)
        {
            return RedirectToPage("/Login");
        }

        await LoadAsync(sponsorId.Value, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var sponsorId = GetSponsorId();
        if (sponsorId == null)
        {
            return RedirectToPage("/Login");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(sponsorId.Value, cancellationToken);
            return Page();
        }

        try
        {
            await _accounts.IncreaseSponsorDonationAsync(sponsorId.Value, Input.OrphanId, Input.Amount, cancellationToken);
            StatusMessage = "Donation increased successfully.";
            return RedirectToPage("/SponsorPortal/Index");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            await LoadAsync(sponsorId.Value, cancellationToken);
            return Page();
        }
    }

    private async Task LoadAsync(int sponsorId, CancellationToken cancellationToken)
    {
        SponsoredOrphans = await _accounts.GetSponsoredOrphansAsync(sponsorId, cancellationToken);
        TotalDonation = await _accounts.GetSponsorTotalDonationAsync(sponsorId, cancellationToken);

        if (SponsoredOrphans.Count > 0 && Input.OrphanId <= 0)
        {
            Input.OrphanId = SponsoredOrphans[0].Id;
        }
    }

    private int? GetSponsorId()
    {
        var claim = User.FindFirst("SponsorId")?.Value;
        return int.TryParse(claim, out var id) ? id : null;
    }

    public sealed class InputModel
    {
        [Range(1, int.MaxValue)]
        public int OrphanId { get; set; }

        [Range(typeof(decimal), "1", "999999999")]
        public decimal Amount { get; set; }
    }
}
