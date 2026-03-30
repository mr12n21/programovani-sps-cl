using Godot;
using System;

public partial class Projectile : Area2D, IPoolable
{
	[Export] public float Speed { get; set; } = 1000;
	public float Damage { get; set; } = 10;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is IDamageable damageable)
		{
			damageable.TakeDamage(Damage);
			Deactivate();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 right = Vector2.Right.Rotated(Rotation);
		Position += right * Speed * (float)delta;
	}

	public void OnScreenExited() => Deactivate();
	public void Init() => Poolable.Init(this);
	public void Activate() => Poolable.Activate(this);
	public void Deactivate() => Poolable.Deactivate(this);
}
