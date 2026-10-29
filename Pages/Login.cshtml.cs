using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrphanageWeb.Data;

namespace OrphanageWeb.Pages;

public class LoginModel : PageModel
{
    private readonly AccountRepository _accounts;

    public LoginModel(AccountRepository accounts)
    {
        _accounts = accounts;
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
            string? displayName;
            string role;

            if (string.Equals(Input.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                role = "Admin";
                displayName = await _accounts.ValidateAdminAsync(Input.Id, Input.Password, cancellationToken);
            }
            else
            {
                role = "Sponsor";
                displayName = await _accounts.ValidateSponsorAsync(Input.Id, Input.Password, cancellationToken);
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                ErrorMessage = "Invalid credentials.";
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, displayName),
                new Claim(ClaimTypes.Role, role),
            };

            if (role == "Admin")
            {
                claims.Add(new Claim("AdminId", Input.Id.ToString()));
            }
            else
            {
                claims.Add(new Claim("SponsorId", Input.Id.ToString()));
            }

            var identity = new ClaimsIdentity(claims, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);

            return role == "Admin"
                ? RedirectToPage("/Index")
                : RedirectToPage("/SponsorPortal/Index");
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
        public string Role { get; set; } = "Admin";

        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; } = string.Empty;
    }
}
