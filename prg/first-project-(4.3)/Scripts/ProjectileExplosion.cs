using Godot;
using System;

public partial class ProjectileExplosion : Area2D, IPoolable
{
	// Called when the node enters the scene tree for the first time.
	AnimationPlayer AnimationPlayer;
	public override void _Ready()
	{
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		AnimationPlayer.AnimationFinished += OnAnimationFinished;
	}

	public void Init()
	{
		Poolable.Init(this);
	}

	public void Activate()
	{
		Poolable.Activate(this);
		AnimationPlayer.Play("Explosion");
	}

	public void Deactivate()
	{
		Poolable.Deactivate(this);
	}

	public void OnAnimationFinished(StringName animationName)
	{
		Deactivate();
	}
}
