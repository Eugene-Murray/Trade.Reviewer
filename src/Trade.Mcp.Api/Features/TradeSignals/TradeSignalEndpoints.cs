using Trade.Application.TradeSignals;

namespace Trade.Mcp.Api.Features.TradeSignals;

internal static class TradeSignalEndpoints
{
    public static RouteGroupBuilder MapTradeSignalEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/trade-signals").WithTags("Trade Signals");

        group.MapGet("/", async (ITradeSignalService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetAllAsync(cancellationToken)));

        group.MapGet("/{id:int}", async (int id, ITradeSignalService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetByIdAsync(id, cancellationToken)));

        group.MapPost("/", async (UpsertTradeSignalRequest request, ITradeSignalService service, CancellationToken cancellationToken) =>
        {
            var created = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/api/trade-signals/{created.Id}", created);
        });

        group.MapPut("/{id:int}", async (int id, UpsertTradeSignalRequest request, ITradeSignalService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.UpdateAsync(id, request, cancellationToken)));

        group.MapDelete("/{id:int}", async (int id, ITradeSignalService service, CancellationToken cancellationToken) =>
        {
            await service.DeleteAsync(id, cancellationToken);
            return Results.NoContent();
        });

        return api;
    }
}
