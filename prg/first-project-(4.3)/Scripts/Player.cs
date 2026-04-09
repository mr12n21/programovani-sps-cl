using Godot;
using System;

public partial class Player : CharacterBody2D, IDamageable
{
	public const float Speed = 300.0f;
	[Export] public Node2D PlayerHead;
	[Export] Gun gun;

	public override void _PhysicsProcess(double delta)
	{
		if (PlayerImage != null) {
			PlayerHead.Rotation = (GetGlobalMousePosition() - GlobalPosition).Angle() - Mathf.Pi / 2;
		}
		Vector2 vector2 = Input.GetVector("Left", "Right", "Up", "Down");
		vector2 = vector2.Normalized();
		Vector2 velocity = Velocity;
		velocity.X = Mathf.Lerp(velocity.X, vector2.X * Speed, 0.1f);
		velocity.Y = Mathf.Lerp(velocity.Y, vector2.Y * Speed, 0.1f);
		Velocity = velocity;
		MoveAndSlide();
	}
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton MouseButtonEvent)
		{
			if(MouseButtonEvent.ButtonIndex == MouseButton.Left && MouseButtonEvent.Pressed)
			{
				gun.Shoot();
			}
		}
    }
[Export]
	public float MaxHealth { get; set; } = 100;
	[Export] public Sprite2D PlayerImage;
    public float Health
	{
		get => _health;
		set
		{
			value = Math.Min(value, MaxHealth);
			if (value == _health) return;
			_health = value;
			OnHealthChanged?.Invoke(_health);
			if (_health <= 0) Die();
		}
	}

	[Export] private float _health = 100;

    public event Action<float> OnHealthChanged;

	private uint _collisionLayer = 1;

	public override void _Ready()
	{
		_collisionLayer = CollisionLayer;
	}

    public void Heal(float hp)
    {
        Health += hp;
    }

    public void TakeDamage(float damage)
    {
		Health -= damage;
    }

	public void Die()
	{
		GD.PushError("!!! YOU DIED !!! YOU GOT SLIMED BY THE 🧃 !!!");
		Game.Instance.GetTree().Quit();
	}

}
