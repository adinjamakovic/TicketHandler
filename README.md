# TicketHandler

A full-stack event ticketing platform: organizers publish events and sell tickets, visitors browse and buy them, and administrators manage the whole catalogue from a dedicated back-office.

The system is split into three runnable applications — an ASP.NET Core Web API, a Duende IdentityServer instance that issues the tokens, and an Angular single-page app that consumes both.

---

## What it does

**Public (no account needed)**
- Browse and search upcoming events, filter by city, event type and date
- Event details page with venue info, an embedded Google Map of the location and event news
- Shareable QR code per event

**Accounts**
- OpenID Connect login / logout / registration, with silent token renewal
- Cart → checkout → order confirmation, with card payment through Stripe
- Domain and API support for wallets, loyalty programmes, reviews and transactions
  (no dedicated UI yet — these are driven from the admin and organizer portals today)

**Organizers**
- Full CRUD for their own events, ticket types and issued tickets
- Publish event news, manage performers and venues
- Organizer profile and settings

**Administrators**
- Manage organizers, events, orders and catalogues
- Global admin news and platform settings

**Platform-wide**
- Card payments — server-priced quote, a Stripe Payment Intent per order and a signature-verified
  webhook that settles the order once Stripe confirms the charge
- AI assistant — an in-app chat widget backed by OpenAI, answering user questions through a
  rate-limited `POST /Ai/chat` endpoint
- Error and performance monitoring via Sentry on both the API and the SPA
- Two languages out of the box (English + Bosnian) via `@ngx-translate`
- Role-driven routing (`admin` / `organizer` / `client` / public portals)
- Soft deletes, paged list endpoints and a uniform error contract across the API

---

## Tech stack

### Backend — `Market.Backend/`

| | |
|---|---|
| Framework | ASP.NET Core 8 (`Market.API`), ASP.NET Core 10 (`Market.IdentityServer`) |
| Language | C# |
| Architecture | Clean Architecture + CQRS |
| Mediator | MediatR 12 |
| Validation | FluentValidation 12 (auto-discovered, run in a MediatR pipeline behaviour) |
| Data access | Entity Framework Core 8 + SQL Server |
| Identity | Duende IdentityServer 8 (authorization code + PKCE), JWT bearer on the API |
| Logging | Serilog (console + rolling file) |
| Monitoring | Sentry (`Sentry.AspNetCore` 6) — errors plus custom request/handler tracing |
| Payments | Stripe.net 52 — Payment Intents and a signed webhook behind `IPaymentGateway` |
| AI | OpenAI Chat Completions via a typed `HttpClient` behind `IAiCompletionService` |
| File storage | Azure Blob Storage (event/venue images) |
| API docs | Swagger / Swashbuckle, wired to IdentityServer for interactive auth |
| Tests | xUnit + `WebApplicationFactory` integration tests |

Projects, with dependencies pointing strictly inward:

```
Market.API              thin controllers, only build a command/query and call MediatR
Market.Application      CQRS commands, queries, handlers, validators
Market.Domain           entities only, no framework dependencies
Market.Infrastructure   EF Core DbContext, configurations, migrations, seeders, external services
Market.Shared           cross-cutting DTOs and options
Market.IdentityServer   Duende IdentityServer host + login UI
Market.Tests            xUnit test project
```

### Frontend — `Market.Frontend/TicketHandler-frontend/`

| | |
|---|---|
| Framework | Angular 21 (NgModule-based, lazy-loaded feature modules) |
| Language | TypeScript 5.9 |
| UI | Angular Material 21 + CDK |
| Auth | `angular-auth-oidc-client` (OIDC code flow + PKCE, rotating refresh tokens) |
| Payments | `@stripe/stripe-js` — Stripe Elements on the checkout page |
| Monitoring | `@sentry/angular` — error reporting, browser tracing and session replay |
| i18n | `@ngx-translate/core` |
| Maps | `@angular/google-maps` |
| QR codes | `ng-qrcode` |
| Reactive | RxJS 7 |

---

## Getting started

### Prerequisites

- **.NET SDK 10** (also builds the `net8.0` projects) and the **.NET 8 runtime**
- **SQL Server** (LocalDB, Developer Edition or a container) reachable on `localhost`
- **Node.js 20.19+ / 22.12+** and npm 11
- A **Google Maps JavaScript API key** (optional — only the map on the event details page needs it)
- An **OpenAI API key** (optional — only the AI chat widget needs it)
- **Stripe test keys** (optional — only checkout needs them; the rest of the app boots without them)
- Two **Sentry DSNs**, one per project (optional — monitoring is skipped when they are empty)

### 1. Clone

```bash
git clone <repo-url>
cd TicketHandler
```

### 2. Create the local config files

Neither the API's `appsettings.json` nor the SPA's `environment.ts` is committed — both carry keys. Each has a committed placeholder beside it, plus a password-protected zip holding the maintainer's own copy (ask the repo owner for the password; a fresh setup doesn't need it):

```bash
# backend — in Market.Backend/Market.API/
cp appsettings.example.json appsettings.json          # or: unzip appsettings.zip

# frontend — in Market.Frontend/TicketHandler-frontend/src/environments/
cp environment-template.ts environment.ts             # or: unzip environment.zip
```

On Windows, use `copy` instead of `cp`.

`Market.IdentityServer/appsettings.json` holds no secrets and *is* committed, so it needs no such step.

### 3. Configure the database

Both backend hosts read the `Main` connection string and default to:

```
Server=localhost;Database=TicketHandler;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

If your SQL Server lives somewhere else, override it in `Market.Backend/Market.API/appsettings.json` and `Market.Backend/Market.IdentityServer/appsettings.json`, or via user secrets.

You do **not** need to run migrations manually — the API applies them on startup and seeds demo data in `Development`. If you prefer to do it by hand:

```bash
cd Market.Backend
dotnet ef database update --project Market.Infrastructure --startup-project Market.API
```

### 4. Configure secrets

Every secret is blank in `appsettings.example.json` and belongs in user secrets:

```bash
cd Market.Backend/Market.API
dotnet user-secrets set "ConnectionStrings:AzureBlob" "<azure-blob-connection-string>"
dotnet user-secrets set "OpenAi:ApiKey"              "<openai-api-key>"
dotnet user-secrets set "Sentry:Dsn"                 "<backend-sentry-dsn>"
dotnet user-secrets set "Stripe:PublishableKey"      "pk_test_..."
dotnet user-secrets set "Stripe:SecretKey"           "sk_test_..."
dotnet user-secrets set "Stripe:WebhookSecret"       "whsec_..."
```

Everything here is optional for a first run: without the blob connection string image upload fails, without the OpenAI key the chat widget returns an error, without the Stripe keys the checkout endpoints refuse (the rest of the API still boots), and without the DSN nothing is reported to Sentry. Non-secret settings — the `OpenAi` base URL, model, system prompt and timeout, `Stripe:Currency`, `ImageCompression`, `IdentityServer` — stay in `appsettings.json`.

`appsettings.json` is git-ignored, so putting the values straight into it works too; user secrets just keep them out of the working tree entirely.

### 5. Trust the local HTTPS certificate

```bash
dotnet dev-certs https --trust
```

### 6. Run IdentityServer — `https://localhost:5001`

```bash
cd Market.Backend
dotnet run --project Market.IdentityServer
```

Start this **first**: both the API and the SPA validate tokens against it.

### 7. Run the API — `https://localhost:7260`

In a second terminal:

```bash
cd Market.Backend
dotnet run --project Market.API
```

Swagger UI is at <https://localhost:7260/swagger>. Use the **Authorize** button to log in through IdentityServer — the Swagger client is pre-registered.

### 8. Run the frontend — `http://localhost:4200`

In a third terminal:

```bash
cd Market.Frontend/TicketHandler-frontend
npm install
npm start
```

Fill in your keys in the `environment.ts` created in step 2; the rest already points at the local backend:

```ts
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7260',
  oidcAuthority: 'https://localhost:5001',
  oidcClientId: 'market.spa',
  oidcScope: 'openid profile email market.api offline_access',
  GApiKey: '<your-google-maps-api-key>',
  SentryDsn: '<your-frontend-sentry-dsn>',
  sentryEnvironment: 'development',
};
```

There is no Stripe key here — the SPA fetches the publishable key from `GET /Payments/config` at checkout.

Open <http://localhost:4200>.

### Demo accounts

Seeded automatically in `Development`:

| Role | Email | Password |
|---|---|---|
| Admin | `admin@market.local` | `Admin123!` |
| Organizer | `dummy_organiser@market.local` | `User123!` |
| Organizer | `dummy_organiser2@market.local` | `User123!` |
| User | `kenan.jamakovic@market.local` | `User123!` |
| User | `merisa.jamakovic@market.local` | `User123!` |

---

## Common commands

**Backend** (from `Market.Backend/`)

```bash
dotnet build Market.Backend.sln
dotnet test Market.Tests/Market.Tests.csproj
dotnet run --project Market.API
dotnet run --project Market.IdentityServer
dotnet ef migrations add <Name> --project Market.Infrastructure --startup-project Market.API
```

**Frontend** (from `Market.Frontend/TicketHandler-frontend/`)

```bash
npm start     # dev server on http://localhost:4200
npm run build # production build
npm run watch # incremental development build
npm test      # Karma/Jasmine
```

---

## Notes

- Ports matter: the OIDC clients in `Market.IdentityServer/Config.cs` and the API's CORS policy are pinned to `http://localhost:4200`, `https://localhost:7260` and `http://localhost:5177`. If you change a port, update both.
- Every API endpoint requires authentication by default (fallback authorization policy); public endpoints opt out with `[AllowAnonymous]`.
- The AI chat endpoint is additionally rate limited to 10 requests per minute per caller (`AiRateLimitPolicy` in `Market.API/DependencyInjection.cs`).
- `Market.API/appsettings.json` and the SPA's `environment.ts` are git-ignored; `appsettings.example.json` and `environment-template.ts` are the committed placeholders to copy, and the `.zip` files beside them are password-protected snapshots of the real config.
- Stripe webhooks: the SPA settles an order by calling `POST /Payments/confirm`, and the webhook is the backstop for when the buyer never comes back (closed tab, redirect payment methods). To exercise it locally run `stripe listen --forward-to https://localhost:7260/Payments/webhook` and set `Stripe:WebhookSecret` to the `whsec_...` it prints.
- Charges are created in `Stripe:Currency` (`BAM` by default), which must match the currency the catalogue prices are expressed in — ticket prices are sent to Stripe as-is.
- Frontend and backend report to **two separate Sentry projects**, so each needs its own DSN.
- The IdentityServer signing key under `Market.IdentityServer/keys/` and the seeded credentials above are for local development only — replace them before deploying anywhere real.
