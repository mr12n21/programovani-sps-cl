using Godot;

public partial class FallingItem : Area2D, IDamageable
{
    public enum ItemType
    {
        Energy,
        Asteroid
    }

    [Signal] public delegate void CollectedEventHandler(int amount);
    [Signal] public delegate void PlayerHitEventHandler(int damage);
    [Signal] public delegate void DestroyedEventHandler(int points);

    [Export] public ItemType Type = ItemType.Energy;
    [Export] public float FallSpeed = 200f;
    [Export] public int EnergyValue = 1;
    [Export] public int AsteroidDamage = 1;
    [Export] public int AsteroidHealth = 2;
    [Export] public int PointsOnDestroy = 2;
    [Export] public float RotationSpeed = 0f;

    private Polygon2D _polygon;
    private int _health;

    public override void _Ready()
    {
        _polygon = GetNode<Polygon2D>("Polygon2D");
        _health = Type == ItemType.Asteroid ? AsteroidHealth : 1;
        UpdateVisual();
        AreaEntered += OnAreaEntered;
    }

    public override void _Process(double delta)
    {
        Position += Vector2.Down * FallSpeed * (float)delta;
        Rotation += RotationSpeed * (float)delta;

        if (Position.Y > GetViewportRect().Size.Y + 40)
        {
            QueueFree();
        }
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area is not Player)
        {
            return;
        }

        if (Type == ItemType.Energy)
        {
            EmitSignal(SignalName.Collected, EnergyValue);
        }
        else
        {
            EmitSignal(SignalName.PlayerHit, AsteroidDamage);
        }

        QueueFree();
    }

    public void TakeHit(int damage)
    {
        if (Type == ItemType.Energy)
        {
            EmitSignal(SignalName.Collected, EnergyValue);
            QueueFree();
            return;
        }

        _health -= damage;
        if (_health <= 0)
        {
            EmitSignal(SignalName.Destroyed, PointsOnDestroy);
            QueueFree();
        }
    }

    public void UpdateVisual()
    {
        if (_polygon == null)
        {
            return;
        }

        _polygon.Color = Type == ItemType.Energy
            ? new Color(0.25f, 0.95f, 1f)
            : new Color(0.9f, 0.6f, 0.3f);
    }
}
