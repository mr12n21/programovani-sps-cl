using Godot;

public partial class Starfield : Node2D
{
    [Export] public int StarCount = 140;
    [Export] public Vector2 SpeedRange = new Vector2(30, 160);
    [Export] public Vector2 SizeRange = new Vector2(1, 3);
    [Export] public Color BaseColor = new Color(0.8f, 0.9f, 1f);
    [Export] public int NebulaCount = 4;
    [Export] public Vector2 NebulaSizeRange = new Vector2(120, 240);
    [Export] public float NebulaAlpha = 0.12f;

    private struct Star
    {
        public Vector2 Position;
        public float Speed;
        public float Size;
        public Color Color;
    }

    private Star[] _stars = System.Array.Empty<Star>();
    private Vector2[] _nebulaPositions = System.Array.Empty<Vector2>();
    private float[] _nebulaSizes = System.Array.Empty<float>();
    private Color[] _nebulaColors = System.Array.Empty<Color>();
    private readonly RandomNumberGenerator _rng = new();

    public override void _Ready()
    {
        _rng.Randomize();
        var size = GetViewportRect().Size;

        _stars = new Star[StarCount];
        for (var i = 0; i < _stars.Length; i++)
        {
            _stars[i] = new Star
            {
                Position = new Vector2(_rng.RandfRange(0, size.X), _rng.RandfRange(0, size.Y)),
                Speed = _rng.RandfRange(SpeedRange.X, SpeedRange.Y),
                Size = _rng.RandfRange(SizeRange.X, SizeRange.Y),
                Color = BaseColor.Lerp(Colors.White, _rng.RandfRange(0f, 0.35f))
            };
        }

        _nebulaPositions = new Vector2[NebulaCount];
        _nebulaSizes = new float[NebulaCount];
        _nebulaColors = new Color[NebulaCount];

        for (var i = 0; i < NebulaCount; i++)
        {
            _nebulaPositions[i] = new Vector2(_rng.RandfRange(0, size.X), _rng.RandfRange(0, size.Y));
            _nebulaSizes[i] = _rng.RandfRange(NebulaSizeRange.X, NebulaSizeRange.Y);
            var tint = new Color(_rng.RandfRange(0.35f, 0.65f), _rng.RandfRange(0.4f, 0.7f), 1f, NebulaAlpha);
            _nebulaColors[i] = tint;
        }
    }

    public override void _Process(double delta)
    {
        var size = GetViewportRect().Size;
        for (var i = 0; i < _stars.Length; i++)
        {
            var star = _stars[i];
            star.Position.Y += star.Speed * (float)delta;

            if (star.Position.Y > size.Y + 4)
            {
                star.Position = new Vector2(_rng.RandfRange(0, size.X), -4);
                star.Speed = _rng.RandfRange(SpeedRange.X, SpeedRange.Y);
                star.Size = _rng.RandfRange(SizeRange.X, SizeRange.Y);
                star.Color = BaseColor.Lerp(Colors.White, _rng.RandfRange(0f, 0.35f));
            }

            _stars[i] = star;
        }

        QueueRedraw();
    }

    public override void _Draw()
    {
        for (var i = 0; i < _nebulaPositions.Length; i++)
        {
            DrawCircle(_nebulaPositions[i], _nebulaSizes[i], _nebulaColors[i]);
        }

        foreach (var star in _stars)
        {
            DrawCircle(star.Position, star.Size, star.Color);
        }
    }
}
