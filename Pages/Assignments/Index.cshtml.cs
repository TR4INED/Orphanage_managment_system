using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrphanageWeb.Data;
using OrphanageWeb.Data.Models;

namespace OrphanageWeb.Pages.Assignments;

public class IndexModel : PageModel
{
    private readonly AssignmentRepository _repository;

    public IndexModel(AssignmentRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyList<AssistedByMapping> AssistedBy { get; private set; } = Array.Empty<AssistedByMapping>();
    public IReadOnlyList<SponsoredByMapping> SponsoredBy { get; private set; } = Array.Empty<SponsoredByMapping>();
    public IReadOnlyList<ServedToMapping> ServedTo { get; private set; } = Array.Empty<ServedToMapping>();

    public IReadOnlyList<LookupItem> VolunteerOptions { get; private set; } = Array.Empty<LookupItem>();
    public IReadOnlyList<LookupItem> SponsorOptions { get; private set; } = Array.Empty<LookupItem>();
    public IReadOnlyList<LookupItem> MealplanOptions { get; private set; } = Array.Empty<LookupItem>();
    public IReadOnlyList<LookupItem> OrphanOptions { get; private set; } = Array.Empty<LookupItem>();

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync(cancellationToken);
            return Page();
        }

        try
        {
            switch (Input.MappingType)
            {
                case MappingType.AssistedBy:
                    await _repository.CreateAssistedByAsync(Input.VolunteerId, Input.OrphanId, cancellationToken);
                    break;
                case MappingType.SponsoredBy:
                    await _repository.CreateSponsoredByAsync(Input.SponsorId, Input.OrphanId, cancellationToken);
                    break;
                case MappingType.ServedTo:
                    await _repository.CreateServedToAsync(Input.MealplanId, Input.OrphanId, cancellationToken);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported mapping type");
            }

            return RedirectToPage("/Assignments/Index");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            await LoadAsync(cancellationToken);
            return Page();
        }
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        try
        {
            AssistedBy = await _repository.GetAssistedByAsync(cancellationToken);
            SponsoredBy = await _repository.GetSponsoredByAsync(cancellationToken);
            ServedTo = await _repository.GetServedToAsync(cancellationToken);

            VolunteerOptions = await _repository.GetVolunteerOptionsAsync(cancellationToken);
            SponsorOptions = await _repository.GetSponsorOptionsAsync(cancellationToken);
            MealplanOptions = await _repository.GetMealplanOptionsAsync(cancellationToken);
            OrphanOptions = await _repository.GetOrphanOptionsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            AssistedBy = Array.Empty<AssistedByMapping>();
            SponsoredBy = Array.Empty<SponsoredByMapping>();
            ServedTo = Array.Empty<ServedToMapping>();

            VolunteerOptions = Array.Empty<LookupItem>();
            SponsorOptions = Array.Empty<LookupItem>();
            MealplanOptions = Array.Empty<LookupItem>();
            OrphanOptions = Array.Empty<LookupItem>();
        }
    }

    public sealed class InputModel
    {
        [Required]
        public MappingType MappingType { get; set; } = MappingType.AssistedBy;

        [Range(1, int.MaxValue)]
        public int VolunteerId { get; set; } = 1;

        [Range(1, int.MaxValue)]
        public int SponsorId { get; set; } = 1;

        [Range(1, int.MaxValue)]
        public int MealplanId { get; set; } = 1;

        [Range(1, int.MaxValue)]
        public int OrphanId { get; set; } = 1;
    }

    public enum MappingType
    {
        AssistedBy,
        SponsoredBy,
        ServedTo,
    }
}
