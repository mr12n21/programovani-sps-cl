using Godot;
using System;

public partial class HealthBar : ProgressBar
{
	[Export] public Node2D Node;
	private IDamageable _damageable;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (Node is IDamageable damageable)
		{
			_damageable = damageable;
			_damageable.OnHealthChanged += OnHealthChanged;
			OnHealthChanged();
		}
	}
	private void OnHealthChanged(float value = 0)
	{
		if (_damageable == null) return;
		MaxValue = _damageable.MaxHealth;
		Value = _damageable.Health;
	}

}
