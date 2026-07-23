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
- Domain and API support for carts, orders, wallets, loyalty programmes, reviews and transactions
  (the logged-in `client` portal itself is still a scaffold — these are driven from the admin and
  organizer portals today)

**Organizers**
- Full CRUD for their own events, ticket types and issued tickets
- Publish event news, manage performers and venues
- Organizer profile and settings

**Administrators**
- Manage organizers, events, orders and catalogues
- Global admin news and platform settings

**Platform-wide**
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
| File storage | Azure Blob Storage (event/venue images) |
| API docs | Swagger / Swashbuckle, wired to IdentityServer for interactive auth |
| Tests | xUnit + `WebApplicationFactory` integration tests |

Projects, with dependencies pointing strictly inward:

```
Market.API              thin controllers, only build a command/query and call MediatR
Market.Application      CQRS commands, queries, handlers, validators
Market.Domain           entities only, no framework dependencies
Market.Infrastructure   EF Core DbContext, configurations, migrations, seeders
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

### 1. Clone

```bash
git clone <repo-url>
cd TicketHandler
```

### 2. Configure the database

Both backend hosts read the `Main` connection string and default to:

```
Server=localhost;Database=TicketHandler;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

If your SQL Server lives somewhere else, override it in `Market.Backend/Market.API/appsettings.json` and `Market.Backend/Market.IdentityServer/appsettings.json`, or via user secrets:

```bash
cd Market.Backend/Market.API
dotnet user-secrets set "ConnectionStrings:Main" "<your-connection-string>"
dotnet user-secrets set "ConnectionStrings:AzureBlob" "<your-azure-blob-connection-string>"
```

You do **not** need to run migrations manually — the API applies them on startup and seeds demo data in `Development`. If you prefer to do it by hand:

```bash
cd Market.Backend
dotnet ef database update --project Market.Infrastructure --startup-project Market.API
```

### 3. Trust the local HTTPS certificate

```bash
dotnet dev-certs https --trust
```

### 4. Run IdentityServer — `https://localhost:5001`

```bash
cd Market.Backend
dotnet run --project Market.IdentityServer
```

Start this **first**: both the API and the SPA validate tokens against it.

### 5. Run the API — `https://localhost:7260`

In a second terminal:

```bash
cd Market.Backend
dotnet run --project Market.API
```

Swagger UI is at <https://localhost:7260/swagger>. Use the **Authorize** button to log in through IdentityServer — the Swagger client is pre-registered.

### 6. Run the frontend — `http://localhost:4200`

In a third terminal:

```bash
cd Market.Frontend/TicketHandler-frontend
cp src/environments/environment-template.ts src/environments/environment.ts   # Windows: copy
npm install
npm start
```

`environment.ts` is not committed. Fill in your Google Maps key; the rest already points at the local backend:

```ts
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7260',
  oidcAuthority: 'https://localhost:5001',
  oidcClientId: 'market.spa',
  oidcScope: 'openid profile email market.api offline_access',
  GApiKey: '<your-google-maps-api-key>',
};
```

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
- The IdentityServer signing key under `Market.IdentityServer/keys/` and the seeded credentials above are for local development only — replace them before deploying anywhere real.
