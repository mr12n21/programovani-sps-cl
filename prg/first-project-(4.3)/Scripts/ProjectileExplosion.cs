using Godot;
using System;

public partial class ProjectileExplosion : Area2D, IPoolable
{
	[Export] public float Lifetime { get; set; } = 0.18f;

	private AnimationPlayer _animationPlayer;
	private SceneTreeTimer _lifetimeTimer;
	private bool _isActive;

	public override void _Ready()
	{
		_animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		if (_animationPlayer != null)
		{
			_animationPlayer.AnimationFinished += OnAnimationFinished;
		}
	}

	public void Init()
	{
		Poolable.Init(this);
	}

	public void Activate()
	{
		Poolable.Activate(this);
		_isActive = true;
		if (_animationPlayer != null && _animationPlayer.HasAnimation("Explosion"))
		{
			_animationPlayer.Play("Explosion");
		}

		StartLifetimeTimer();
	}

	public void Deactivate()
	{
		if (!_isActive)
		{
			return;
		}

		_isActive = false;
		_lifetimeTimer = null;
		_animationPlayer?.Stop();
		Poolable.Deactivate(this);
	}

	public void OnAnimationFinished(StringName animationName)
	{
		Deactivate();
	}

	private async void StartLifetimeTimer()
	{
		var timer = GetTree().CreateTimer(Lifetime);
		_lifetimeTimer = timer;
		await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
		if (_lifetimeTimer == timer)
		{
			Deactivate();
		}
	}
}
