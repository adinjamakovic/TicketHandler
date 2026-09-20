using System.ComponentModel.DataAnnotations;

namespace Market.IdentityServer.Pages.Login;

public class InputModel
{
    // Credentials are looked up by email (see PersonCredentialStore), so the field is
    // labelled for what it actually takes.
    [Required]
    [Display(Name = "Email")]
    public string? Username { get; set; }
    [Required]
    public string? Password { get; set; }
    public bool RememberLogin { get; set; }
    public string? ReturnUrl { get; set; }
    public string? Button { get; set; }
}
