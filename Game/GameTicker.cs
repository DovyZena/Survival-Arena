using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace MyGame;

public class GameTicker : BackgroundService
{
    private readonly GameState _state;
    private readonly IHubContext<MyGame.Hubs.GameHub> _hub;

    public GameTicker(GameState state, IHubContext<MyGame.Hubs.GameHub> hub)
    {
        _state = state;
        _hub = hub;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var last = sw.Elapsed;
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = sw.Elapsed;
            var delta = (float)(now - last).TotalSeconds;
            last = now;
            _state.Tick(delta);
            var snap = _state.GetSnapshot();
            await _hub.Clients.All.SendAsync("State", snap, cancellationToken: stoppingToken);
            await Task.Delay(33, stoppingToken);
        }
    }
}
