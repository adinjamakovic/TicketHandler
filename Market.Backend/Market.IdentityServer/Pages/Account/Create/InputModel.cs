using System.ComponentModel.DataAnnotations;

namespace Market.IdentityServer.Pages.Create;

/// <summary>
/// Self-registration form. It carries no role flags on purpose - the server always
/// creates a plain user, and roles are granted through the admin-only API flow.
/// </summary>
public class InputModel
{
    [Required]
    [Display(Name = "First name")]
    public string? FirstName { get; set; }

    [Required]
    [Display(Name = "Last name")]
    public string? LastName { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Date of birth")]
    public DateTime? BirthDate { get; set; }

    [Required]
    [Display(Name = "City")]
    public int? CityId { get; set; }

    [Required]
    public string? Address { get; set; }

    [Required]
    public string? Gender { get; set; }

    [Required]
    public string? Phone { get; set; }

    [Required]
    public string? Username { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    public string? Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string? ConfirmPassword { get; set; }

    public string? ReturnUrl { get; set; }

    public string? Button { get; set; }
}
