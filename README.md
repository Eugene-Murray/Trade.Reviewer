# Trade.Reviewer

.NET 10 trade review solution with a Minimal API, EF Core + SQLite persistence, and a Blazor dashboard for CRUD management of accounts, watchlist entries, trade signals, and trades.

## Projects

| Path | Purpose |
| --- | --- |
| `src/Trade.Domain` | Core entities and enums |
| `src/Trade.Application` | DTOs, service contracts, exceptions, and trade performance logic |
| `src/Trade.Infrastructure` | EF Core DbContext, migrations, seeding, and service implementations |
| `src/Trade.Mcp.Api` | Minimal API host, logging, health checks, and exception handling |
| `src/Trade.Dashboard/Trade.Dashboard` | Blazor Web App host with SSR + interactive render modes |
| `src/Trade.Dashboard/Trade.Dashboard.Client` | Interactive Server/WebAssembly pages and API client |
| `tests/*` | Domain, application, and API tests |

## Data model

- `accounts`
- `watchlist`
- `trade_signals`
- `trades`

The seeded database contains:

- 3 accounts
- 50 Nasdaq mega-cap watchlist entries
- 8 long/short trade signals
- 15 open trades
- 150 closed trades

## Run locally

```powershell
dotnet restore Trade.Reviewer.slnx
dotnet build Trade.Reviewer.slnx
dotnet run --project src\Trade.Mcp.Api\Trade.Mcp.Api.csproj
dotnet run --project src\Trade.Dashboard\Trade.Dashboard\Trade.Dashboard.csproj
```

Default local URLs:

- API: `https://localhost:7091`
- Dashboard: `https://localhost:7187`

## Test

```powershell
dotnet test Trade.Reviewer.slnx
```
