using System.Globalization;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using FluentValidation;
using Market.Application.Common;
using Market.Application.Common.Exceptions;
using Market.Application.Modules.Geography.Cities.Queries.List;
using Market.Application.Modules.Identity.Person.Commands.Create;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Market.IdentityServer.Pages.Create;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    // One page is far more than the catalogue holds, and the loop below picks up the rest.
    private const int CityPageSize = 100;

    private readonly ISender _sender;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IEventService _events;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public IReadOnlyList<ListCitiesQueryDto> Cities { get; private set; } = [];

    public static IReadOnlyList<string> Genders { get; } = ["Male", "Female", "Prefer not to say"];

    public Index(
        ISender sender,
        IIdentityServerInteractionService interaction,
        IEventService events)
    {
        _sender = sender;
        _interaction = interaction;
        _events = events;
    }

    public async Task<IActionResult> OnGetAsync(string? returnUrl, CancellationToken ct)
    {
        Input = new InputModel { ReturnUrl = returnUrl };
        await LoadCitiesAsync(ct);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        // check if we are in the context of an authorization request
        var context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl, ct);

        // the user clicked the "cancel" button
        if (Input.Button != "create")
        {
            if (context != null)
            {
                // if the user cancels, send a result back into IdentityServer as if they
                // denied the consent (even if this client does not require consent).
                // this will send back an access denied OIDC error response to the client.
                await _interaction.DenyAuthorizationAsync(context, InteractionError.AccessDenied, ct);

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(Input.ReturnUrl);
                }

                return Redirect(Input.ReturnUrl ?? "~/");
            }
            else
            {
                // since we don't have a valid context, then we just go back to the home page
                return Redirect("~/");
            }
        }

        if (ModelState.IsValid)
        {
            var command = new CreatePersonCommand
            {
                FirstName = Input.FirstName!.Trim(),
                LastName = Input.LastName!.Trim(),
                BirthDate = Input.BirthDate!.Value,
                CityId = Input.CityId!.Value,
                Address = Input.Address!.Trim(),
                Gender = Input.Gender!,
                Phone = Input.Phone!.Trim(),
                Username = Input.Username!.Trim(),
                Email = Input.Email!.Trim(),
                Password = Input.Password!
            };

            int personId;

            try
            {
                personId = await _sender.Send(command, ct);
            }
            catch (ValidationException ex)
            {
                foreach (var failure in ex.Errors)
                    ModelState.AddModelError(string.Empty, failure.ErrorMessage);

                await LoadCitiesAsync(ct);
                return Page();
            }
            catch (MarketConflictException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                await LoadCitiesAsync(ct);
                return Page();
            }

            var subjectId = personId.ToString(CultureInfo.InvariantCulture);
            var displayName = command.Email;

            await _events.RaiseAsync(
                new UserLoginSuccessEvent(displayName, subjectId, displayName, clientId: context?.Client.ClientId), ct);
            Telemetry.Metrics.UserLogin(context?.Client.ClientId, IdentityServerConstants.LocalIdentityProvider);

            // issue authentication cookie with subject ID and username
            var isuser = new IdentityServerUser(subjectId)
            {
                DisplayName = displayName
            };

            await HttpContext.SignInAsync(isuser);

            if (context != null)
            {
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(Input.ReturnUrl);
                }

                // we can trust Input.ReturnUrl since GetAuthorizationContextAsync returned non-null
                return Redirect(Input.ReturnUrl ?? "~/");
            }

            // request for a local page
            if (Url.IsLocalUrl(Input.ReturnUrl))
            {
                return Redirect(Input.ReturnUrl);
            }
            else if (string.IsNullOrEmpty(Input.ReturnUrl))
            {
                return Redirect("~/");
            }
            else
            {
                // user might have clicked on a malicious link - should be logged
                throw new ArgumentException("invalid return URL");
            }
        }

        await LoadCitiesAsync(ct);
        return Page();
    }

    private async Task LoadCitiesAsync(CancellationToken ct)
    {
        var cities = new List<ListCitiesQueryDto>();
        var page = 1;

        while (true)
        {
            var result = await _sender.Send(
                new ListCitiesQuery { Paging = new PageRequest { Page = page, PageSize = CityPageSize } }, ct);

            cities.AddRange(result.Items);

            if (page >= result.TotalPages || result.Items.Count == 0)
                break;

            page++;
        }

        Cities = cities;
    }
}
