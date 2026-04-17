using Trade.Application.Trades;

namespace Trade.Mcp.Api.Features.Trades;

internal static class TradeEndpoints
{
    public static RouteGroupBuilder MapTradeEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/trades").WithTags("Trades");

        group.MapGet("/", async (ITradeService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetAllAsync(cancellationToken)));

        group.MapGet("/{id:int}", async (int id, ITradeService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetByIdAsync(id, cancellationToken)));

        group.MapPost("/", async (UpsertTradeRequest request, ITradeService service, CancellationToken cancellationToken) =>
        {
            var created = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/api/trades/{created.Id}", created);
        });

        group.MapPut("/{id:int}", async (int id, UpsertTradeRequest request, ITradeService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.UpdateAsync(id, request, cancellationToken)));

        group.MapDelete("/{id:int}", async (int id, ITradeService service, CancellationToken cancellationToken) =>
        {
            await service.DeleteAsync(id, cancellationToken);
            return Results.NoContent();
        });

        return api;
    }
}
