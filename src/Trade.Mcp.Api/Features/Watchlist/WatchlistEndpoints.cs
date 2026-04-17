using Trade.Application.Watchlist;

namespace Trade.Mcp.Api.Features.Watchlist;

internal static class WatchlistEndpoints
{
    public static RouteGroupBuilder MapWatchlistEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/watchlist").WithTags("Watchlist");

        group.MapGet("/", async (IWatchlistService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetAllAsync(cancellationToken)));

        group.MapGet("/{id:int}", async (int id, IWatchlistService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetByIdAsync(id, cancellationToken)));

        group.MapPost("/", async (UpsertWatchlistItemRequest request, IWatchlistService service, CancellationToken cancellationToken) =>
        {
            var created = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/api/watchlist/{created.Id}", created);
        });

        group.MapPut("/{id:int}", async (int id, UpsertWatchlistItemRequest request, IWatchlistService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.UpdateAsync(id, request, cancellationToken)));

        group.MapDelete("/{id:int}", async (int id, IWatchlistService service, CancellationToken cancellationToken) =>
        {
            await service.DeleteAsync(id, cancellationToken);
            return Results.NoContent();
        });

        return api;
    }
}
