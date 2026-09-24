
namespace MyGame;

public class Player
{
    public int Id { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float VX { get; set; }
    public float VY { get; set; }
    public string Color { get; set; } = "Blue";
}

public class GameState
{
    private readonly object _lock = new();
    public Dictionary<int, Player> Players { get; } = new();
    public Dictionary<string, int> ConnToPlayer { get; } = new();
    private int _nextPlayerId = 1;
    public int Width { get; } = 800;
    public int Height { get; } = 600;
    public float Speed { get; } = 200f;

    public int AddOrGetPlayer(string connectionId)
    {
        lock (_lock)
        {
            if (ConnToPlayer.TryGetValue(connectionId, out var pid)) return pid;
            if (Players.Count >= 3) return 0;
            var id = _nextPlayerId++;
            var p = new Player { Id = id, X = id == 1 ? 100 : id == 2 ? 700 : 500, Y = 300, VX = 0, VY = 0, Color = id == 1 ? "Blue" : id == 2 ? "Red" : "Green" };
            Players[id] = p;
            ConnToPlayer[connectionId] = id;
            return id;
        }
    }

    public void RemovePlayer(string connectionId)
    {
        lock (_lock)
        {
            if (ConnToPlayer.TryGetValue(connectionId, out var pid))
            {
                ConnToPlayer.Remove(connectionId);
                Players.Remove(pid);
            }
        }
    }

    public void SetInput(string connectionId, float vx, float vy)
    {
        lock (_lock)
        {
            if (!ConnToPlayer.TryGetValue(connectionId, out var pid)) return;
            if (Players.TryGetValue(pid, out var p))
            {
                p.VX = vx * Speed;
                p.VY = vy * Speed;
            }
        }
    }

    public Snapshot GetSnapshot()
    {
        lock (_lock)
        {
            return new Snapshot
            {
                Players = Players.Values.Select(p => new PlayerSnapshot { Id = p.Id, X = p.X, Y = p.Y, Color = p.Color }).ToList()
            };
        }
    }

    public void Tick(float seconds)
    {
        lock (_lock)
        {
            foreach (var p in Players.Values)
            {
                p.X += p.VX * seconds;
                p.Y += p.VY * seconds;
                p.X = (float)Math.Clamp(p.X, 0, Width - 50);
                p.Y = (float)Math.Clamp(p.Y, 0, Height - 50);
            }
        }
    }

    public class Snapshot
    {
        public List<PlayerSnapshot> Players { get; set; } = new();
    }

    public class PlayerSnapshot
    {
        public int Id;
        public float X;
        public float Y;
        public string Color = "Blue";
    }
}
