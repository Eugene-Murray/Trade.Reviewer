# Architecture

## Overview

Trade.Reviewer uses a modular .NET 10 solution structure with a thin Minimal API host, application contracts/services, EF Core infrastructure, and a Blazor dashboard that consumes the API for all CRUD operations.

## Solution layout

```text
/src
  /Trade.Mcp.Api
  /Trade.Application
  /Trade.Domain
  /Trade.Infrastructure
  /Trade.Dashboard
/tests
  /Trade.Mcp.Api.Tests
  /Trade.Application.Tests
  /Trade.Domain.Tests
Directory.Build.props
Directory.Packages.props
Trade.Reviewer.slnx
```

## Layer responsibilities

- `Trade.Domain`: entities and enums only.
- `Trade.Application`: DTOs, request models, exceptions, and service interfaces.
- `Trade.Infrastructure`: SQLite-backed EF Core implementation, migrations, deterministic seed data, and service implementations.
- `Trade.Mcp.Api`: HTTP endpoints, health checks, CORS, Serilog, and centralized exception handling.
- `Trade.Dashboard`: Blazor UI that calls the API rather than reaching into the database directly.

## Data relationships

- `accounts -> trades` uses `account_id`.
- `watchlist -> trade_signals` uses `stock_name` as an alternate key.
- `watchlist -> trades` uses `stock_name` as an alternate key.

This preserves the requested schema while still giving EF Core navigation properties between the tables.

## UI render modes

- Home page: SSR
- Accounts and trades pages: Interactive Server
- Watchlist and trade signals pages: Interactive WebAssembly

## Local development flow

1. Start `Trade.Mcp.Api`.
2. Start `Trade.Dashboard`.
3. The API applies migrations and seeds the SQLite `trade_dashboard` database on startup.

## Quality gates

- Nullable reference types enabled
- Warnings treated as errors
- Central package management via `Directory.Packages.props`
- xUnit coverage for domain, application, and API layers
- GitHub Actions workflow builds and tests the solution
