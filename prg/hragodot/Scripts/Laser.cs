using Godot;

public partial class Laser : Area2D
{
    [Export] public float Speed = 900f;
    [Export] public int Damage = 1;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
    }

    public override void _Process(double delta)
    {
        Position += Vector2.Up * Speed * (float)delta;

        if (Position.Y < -40)
        {
            QueueFree();
        }
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area is Player)
        {
            return;
        }

        if (area is IDamageable damageable)
        {
            damageable.TakeHit(Damage);
            QueueFree();
        }
    }
}
