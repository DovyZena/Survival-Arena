using MyGame.Hubs;
using System.Drawing;
using System.Drawing.Imaging;
using MyGame;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<GameState>();
builder.Services.AddHostedService<GameTicker>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHub<GameHub>("/gameHub");

app.MapGet("/frame", (GameState state) =>
{
	var snap = state.GetSnapshot();
	using var bmp = new Bitmap(state.Width, state.Height);
	using var g = Graphics.FromImage(bmp);
	g.Clear(Color.White);
	foreach (var p in snap.Players)
	{
		using var brush = new SolidBrush(Color.FromName(p.Color));
		g.FillRectangle(brush, p.X, p.Y, 50, 50);
	}
	using var ms = new MemoryStream();
	bmp.Save(ms, ImageFormat.Png);
	ms.Seek(0, SeekOrigin.Begin);
	return Results.File(ms.ToArray(), "image/png");
});

app.Run();

