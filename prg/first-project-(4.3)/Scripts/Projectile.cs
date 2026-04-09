using Godot;
using System;

public partial class Projectile : Area2D, IPoolable
{
	[Export] public float Speed { get; set; } = 1000;

	[Export] public float Damage { get; set; } = 10;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}
	private void OnBodyEntered(Node2D body)
	{
		if (body is IDamageable enemy)
		{
			ProjectileExplode();
			enemy.TakeDamage(Damage);
			CallDeferred(nameof(Deactivate));
		}
	}

	public void ProjectileExplode()
	{
		ProjectileExplosion explosion = Game.Instance.GetPoolObject<ProjectileExplosion>();
		if (explosion == null) return;
		explosion.GlobalPosition = GlobalPosition;
		explosion.Activate();
		CallDeferred(nameof(Deactivate));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector2 dir = Vector2.Right.Rotated(Rotation);
		Position += dir * Speed * (float)delta;
	}

	public void OnScreenExited()
	{
		Deactivate();
	}

    public void Init()
    {
        Poolable.Init(this);
    }

    public void Activate()
    {
		Poolable.Activate(this);
    }

    public void Deactivate()
    {
		Poolable.Deactivate(this);
    }

}
