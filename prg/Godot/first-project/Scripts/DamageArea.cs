using Godot;
using System;

public partial class DamageArea : Area2D
{
	[Export] public float DamagePerTick = 8;
	[Export] public float TickInterval = 1.0f;

	private Timer _timer = new Timer();

	public override void _Ready()
	{
		_timer.WaitTime = TickInterval;
		_timer.OneShot = false;
		_timer.Timeout += OnTimeout;
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
		AddChild(_timer);
	}

	private void OnBodyEntered(Node2D body)
	{
		// Immediate damage on contact
		if (body is IDamageable damageable)
		{
			damageable.TakeDamage(DamagePerTick);
			if (_timer.IsStopped()) _timer.Start();
		}
	}

	private void OnBodyExited(Node2D body)
	{
		if (!HasOverlappingBodies())
			_timer.Stop();
	}

	private void OnTimeout()
	{
		foreach (var body in GetOverlappingBodies())
		{
			if (body is IDamageable damageable)
				damageable.TakeDamage(DamagePerTick);
		}
	}
}