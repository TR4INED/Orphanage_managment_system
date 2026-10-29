using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrphanageWeb.Data;

namespace OrphanageWeb.Pages.Account;

public class ChangePasswordModel : PageModel
{
    private readonly AccountRepository _accounts;

    public ChangePasswordModel(AccountRepository accounts)
    {
        _accounts = accounts;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; private set; }

    [TempData]
    public string? StatusMessage { get; set; }

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
            var isAdmin = User.IsInRole("Admin");
            var isSponsor = User.IsInRole("Sponsor");

            if (!isAdmin && !isSponsor)
            {
                return RedirectToPage("/Login");
            }

            var changed = false;

            if (isAdmin)
            {
                var adminId = GetClaimInt("AdminId");
                if (adminId == null)
                {
                    return RedirectToPage("/Login");
                }

                changed = await _accounts.ChangeAdminPasswordAsync(adminId.Value, Input.CurrentPassword, Input.NewPassword, cancellationToken);
            }
            else
            {
                var sponsorId = GetClaimInt("SponsorId");
                if (sponsorId == null)
                {
                    return RedirectToPage("/Login");
                }

                changed = await _accounts.ChangeSponsorPasswordAsync(sponsorId.Value, Input.CurrentPassword, Input.NewPassword, cancellationToken);
            }

            if (!changed)
            {
                ErrorMessage = "Current password is incorrect.";
                return Page();
            }

            StatusMessage = "Password updated successfully.";
            return RedirectToPage("/Account/ChangePassword");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }

    private int? GetClaimInt(string claimType)
    {
        var value = User.FindFirst(claimType)?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }

    public sealed class InputModel
    {
        [Required]
        [StringLength(50)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
