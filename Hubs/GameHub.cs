using Microsoft.AspNetCore.SignalR;
using MyGame;

namespace MyGame.Hubs;

public class GameHub : Hub
{
	private readonly GameState _state;

	public GameHub(GameState state)
	{
		_state = state;
	}

	public int Join()
	{
		return _state.AddOrGetPlayer(Context.ConnectionId);
	}

	public override Task OnDisconnectedAsync(Exception? exception)
	{
		_state.RemovePlayer(Context.ConnectionId);
		return base.OnDisconnectedAsync(exception);
	}

	public Task SendInput(float vx, float vy)
	{
		_state.SetInput(Context.ConnectionId, vx, vy);
		return Task.CompletedTask;
	}
}