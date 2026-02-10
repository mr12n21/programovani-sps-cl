using Godot;

public partial class Player : Area2D
{
    [Export] public float Speed = 520f;
    [Export] public float FireCooldown = 0.2f;

    [Signal] public delegate void ShootEventHandler(Vector2 position);

    private float _halfWidth = 36f;
    private float _cooldown;

    public override void _Ready()
    {
        if (GetNodeOrNull<CollisionShape2D>("CollisionShape2D")?.Shape is RectangleShape2D rect)
        {
            _halfWidth = rect.Size.X / 2f;
        }
    }

    public override void _Process(double delta)
    {
        _cooldown -= (float)delta;

        var dir = 0f;
        if (Input.IsActionPressed("ui_left"))
        {
            dir -= 1f;
        }
        if (Input.IsActionPressed("ui_right"))
        {
            dir += 1f;
        }

        var pos = Position;
        pos.X += dir * Speed * (float)delta;

        var viewportWidth = GetViewportRect().Size.X;
        pos.X = Mathf.Clamp(pos.X, _halfWidth, viewportWidth - _halfWidth);

        Position = pos;

        if (_cooldown <= 0f && (Input.IsActionPressed("ui_accept") || Input.IsActionPressed("ui_select")))
        {
            _cooldown = FireCooldown;
            EmitSignal(SignalName.Shoot, Position + new Vector2(0, -24));
        }
    }
}
