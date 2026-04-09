using Godot;
using System;

public partial class DamageArea : Area2D
{
	private Timer _timer = new Timer();
	public override void _Ready()
	{
		_timer.WaitTime = 2;
		_timer.OneShot = false;
		_timer.Timeout += OnTimeout;
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
		AddChild(_timer);
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is IDamageable && _timer.IsStopped()) _timer.Start();
	}

	private void OnBodyExited(Node2D body)
	{
		if(!HasOverlappingBodies())  _timer.Stop();
	}

	private void OnTimeout ()
	{
		foreach (var body in GetOverlappingBodies())
		{
			if (body is IDamageable damageable)
			{
				damageable.TakeDamage(5);
			}
		}
	}
}
