using Trade.Application.Accounts;

namespace Trade.Mcp.Api.Features.Accounts;

internal static class AccountEndpoints
{
    public static RouteGroupBuilder MapAccountEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/accounts").WithTags("Accounts");

        group.MapGet("/", async (IAccountService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetAllAsync(cancellationToken)));

        group.MapGet("/{id:int}", async (int id, IAccountService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetByIdAsync(id, cancellationToken)));

        group.MapPost("/", async (UpsertAccountRequest request, IAccountService service, CancellationToken cancellationToken) =>
        {
            var created = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/api/accounts/{created.Id}", created);
        });

        group.MapPut("/{id:int}", async (int id, UpsertAccountRequest request, IAccountService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.UpdateAsync(id, request, cancellationToken)));

        group.MapDelete("/{id:int}", async (int id, IAccountService service, CancellationToken cancellationToken) =>
        {
            await service.DeleteAsync(id, cancellationToken);
            return Results.NoContent();
        });

        return api;
    }
}
