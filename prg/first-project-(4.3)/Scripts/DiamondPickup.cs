using Godot;

public partial class DiamondPickup : Area2D
{
	[Export] public int Value { get; private set; } = 1;
	[Export] public float HoverSpeed { get; set; } = 2.8f;
	[Export] public float HoverAmplitude { get; set; } = 8f;
	[Export] public float MagnetDistance { get; set; } = 170f;
	[Export] public float MagnetSpeed { get; set; } = 420f;

	private float _time;
	private Player _player;
	private Node2D _visual;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		_visual = GetNodeOrNull<Node2D>("Visual");
	}

	public override void _PhysicsProcess(double delta)
	{
		_time += (float)delta;
		_player ??= Game.Instance?.GetNodeOrNull<Player>("Player");

		if (_player != null && GlobalPosition.DistanceTo(_player.GlobalPosition) <= MagnetDistance)
		{
			GlobalPosition = GlobalPosition.MoveToward(_player.GlobalPosition, MagnetSpeed * (float)delta);
		}

		if (_visual != null)
		{
			_visual.Position = new Vector2(0f, Mathf.Sin(_time * HoverSpeed) * HoverAmplitude);
			_visual.Rotation += (float)delta * 1.4f;
		}
	}

	public void SetValue(int value)
	{
		Value = Mathf.Max(1, value);
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is not Player)
		{
			return;
		}

		Collect();
	}

	private void Collect()
	{
		Game.Instance?.GetNodeOrNull<WeaponInventory>("WeaponInventory")?.AddDiamonds(Value);
		QueueFree();
	}
}
