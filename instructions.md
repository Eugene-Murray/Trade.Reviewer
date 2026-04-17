1. Create a .net 10 solution called 'Trade.Reviewer' using Directory.Packages.props

2. Add Web api project 'Trade.Mcp.Api'.

2. Use Minimal APIs that use service class implementation for the database access and logic.

3. Use entity framework with SQLite Database called 'trade_dashboard' with navigation properties between the tables.

4. Create tables called 'watchlist', 'trades' 'trade-signals' and 'accounts';

watchlist table;
id
stock name
notes
data added

trade signals table;
id
stock name
direction -> long or short
signal date


trades table;
id
stock name
entry price
current price
close price
position size
open date
close date
account id

account table;
id
account name -> trading account, isa account, sipp pension
account balance

5. Seed the tables with US Nasdaq mega cap stocks.
- watchlist table -> 50 stocks in the watchlist table.
- trade signals table -> 8 signals in signal table long and short
- trades table -> 15 open trades in trades table, some in profit, some in negative. 150 closed trades, with mixture of profitable and losing trades
- account table -> trade account, isa account, sipp pension account

6. Add Minimal API endpoints to perform crud operations on the database tables

7. Add a asp.net Blazor Web App with (SSR + Interactive Server + WebAssembly) called 'Trade.Dashboard' that has screens to perform crud operations on the API endpoints

8. Use the following best‑practices checklist** for setting up a new .NET solution;

# ⭐ **1. Start with a clean, modular solution structure**
A predictable layout prevents chaos later.

### Recommended structure
```
/src
   /Api               → ASP.NET host (Minimal APIs or Controllers)
   /Application       → Use cases, services, validators
   /Domain            → Entities, value objects, interfaces
   /Infrastructure    → EF Core, Dapper, external services
/tests
   /Api.Tests
   /Application.Tests
   /Domain.Tests
Directory.Packages.props
MySolution.sln
```

### Why this matters
- Keeps boundaries clean  
- Makes refactoring painless  
- Supports vertical slice or clean architecture  
- Works for microservices or monoliths  

---

# ⭐ **2. Use Central Package Management**
You already asked about this — and yes, it’s essential.

`Directory.Packages.props` at the root:
- One place for all package versions  
- Zero duplication  
- No version drift  

This is a must for multi‑project solutions.

---

# ⭐ **3. Use nullable reference types + analyzers from day one**
Turn these on immediately:

```xml
<Nullable>enable</Nullable>
<TreatWarningsAsErrors>true</TreatWarningsAsErrors>
```

Add analyzers:
- `Microsoft.CodeAnalysis.NetAnalyzers`
- `StyleCop.Analyzers` (optional but great for consistency)

This prevents 80% of future bugs.

---

# ⭐ **4. Adopt a consistent naming + folder convention**
For .NET:
- **PascalCase** for projects, namespaces, classes  
- **camelCase** for fields  
- **snake_case** for SQL tables (if using SQL Server)  
- **PascalCase** for solution name  

For Blazor:
- Components in `Components/FeatureName/`  
- Pages in `Pages/`  
- Shared UI in `Shared/`  

---

# ⭐ **5. Use dependency injection properly**
Avoid service‑locator patterns or static helpers.

Good:
```csharp
builder.Services.AddScoped<IUserService, UserService>();
```

Bad:
```csharp
ServiceLocator.Get<IUserService>();
```

Also:
- Prefer constructor injection  
- Avoid injecting `IConfiguration` everywhere  
- Use options pattern (`IOptions<T>`) for config  

---

# ⭐ **6. Keep your API project thin**
Your API should only:
- Handle HTTP  
- Map requests to application commands/queries  
- Return results  

Everything else belongs in:
- Application layer (business logic)  
- Infrastructure (data, external services)  

This keeps your API clean and testable.

---

# ⭐ **7. Use vertical slice architecture for features**
Instead of giant folders like:
```
/Services
/Models
/Controllers
```

Use feature folders:
```
/Features
   /Users
      GetUser.cs
      CreateUser.cs
      UserController.cs
```

This scales beautifully.

---

# ⭐ **8. Add health checks + structured logging**
Use:
- **Serilog** (with JSON output)
- **HealthChecks** for DB, Redis, external APIs
- **OpenTelemetry** if you want tracing

This gives you observability from day one.

---

# ⭐ **9. Use EF Core migrations correctly**
- Keep migrations in the Infrastructure project  
- Use a dedicated DbContext per bounded context  
- Avoid giant God‑contexts  

If you use Dapper, keep SQL in `/Infrastructure/Sql/`.

---

# ⭐ **10. Add automated tests early**
At minimum:
- Unit tests for domain + application  
- Integration tests for API endpoints  
- Test containers for DB integration (optional but powerful)

This prevents regressions as your solution grows.

---

# ⭐ **11. Use environment‑specific configuration**
Use:
- `appsettings.json`
- `appsettings.Development.json`
- `appsettings.Production.json`

Never hardcode secrets.  
Use:
- Azure Key Vault  
- User secrets in development  

---

# ⭐ **12. Add CI/CD from the start**
Even a simple GitHub Actions pipeline:
- Build  
- Test  
- Publish artifacts  

This keeps your solution healthy.

---

# ⭐ **13. Document your architecture**
A simple `ARCHITECTURE.md` at the root:
- Project structure  
- Naming conventions  
- How layers interact  
- How to run the solution  

This saves future you a lot of pain.

---

# ⭐ **14. Use Minimal APIs for small endpoints**
But keep complex logic in Application layer.

Minimal APIs + Clean Architecture = perfect balance.

---

# ⭐ **15. Use Centralized Error Handling**
Add a global exception handler middleware:
- Converts exceptions to consistent responses  
- Logs properly  
- Avoids duplicated try/catch blocks  


9. Add .net gitignore to the projects


















