using Godot;

public partial class EnemyShip : Area2D, IDamageable
{
    [Signal] public delegate void DestroyedEventHandler(int points);
    [Signal] public delegate void PlayerHitEventHandler(int damage);

    [Export] public float Speed = 140f;
    [Export] public float SwayAmplitude = 50f;
    [Export] public float SwayFrequency = 2.2f;
    [Export] public int Health = 2;
    [Export] public int Damage = 1;
    [Export] public int Points = 4;

    private float _time;
    private float _baseX;

    public override void _Ready()
    {
        _baseX = Position.X;
        AreaEntered += OnAreaEntered;
    }

    public override void _Process(double delta)
    {
        _time += (float)delta;
        var pos = Position;
        pos.Y += Speed * (float)delta;
        pos.X = _baseX + Mathf.Sin(_time * SwayFrequency) * SwayAmplitude;
        Position = pos;

        if (Position.Y > GetViewportRect().Size.Y + 60)
        {
            QueueFree();
        }
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area is Player)
        {
            EmitSignal(SignalName.PlayerHit, Damage);
            QueueFree();
        }
    }

    public void TakeHit(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            EmitSignal(SignalName.Destroyed, Points);
            QueueFree();
        }
    }
}
