using Godot;
using System;

public partial class Enemy : CharacterBody2D, IDamageable, IPoolable
{
	[Export] public Node2D Target;
	[Export] protected NavigationAgent2D Agent;
	[Export] protected float Speed = 50;
	[Export] protected Node2D Head;
	[Export] public float MaxHealth { get; set; } = 100;
	[Export] public int ScoreValue { get; set; } = 100;

	public float DifficultyMultiplier { get; set; } = 1.0f;

	public float Health
	{
		get => _health;
		set
		{
			float max = MaxHealth * DifficultyMultiplier;
			value = Math.Clamp(value, 0, max);
			if (Math.Abs(value - _health) < 0.001f) return;
			_health = value;
			OnHealthChanged?.Invoke(_health);
			if (_health <= 0) Die();
		}
	}

	private float _health = 100;
	public event Action<float> OnHealthChanged;

	public void Die()
	{
		Game.Instance?.OnEnemyKilled(this);
		Deactivate();
	}

	public void Heal(float amount) => Health += amount;

	public void TakeDamage(float damage)
	{
		Health -= damage;
		// Flash red on hit
		if (Head?.GetNodeOrNull<Sprite2D>("Sprite2D") is Sprite2D sprite)
		{
			var tween = CreateTween();
			tween.TweenProperty(sprite, "modulate", new Color(1, 0.2f, 0.2f), 0.05f);
			tween.TweenProperty(sprite, "modulate", Colors.White, 0.15f);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Target == null) return;
		Head.Rotation = (Target.GlobalPosition - GlobalPosition).Angle() - Mathf.Pi / 2;
		Agent.SetTargetPosition(Target.GlobalPosition);
		Vector2 velocity = (Agent.GetNextPathPosition() - GlobalPosition).Normalized() * Speed * DifficultyMultiplier;
		Velocity = velocity;
		MoveAndSlide();
	}

	public virtual void Init()
	{
		Poolable.Init(this);
	}

	public virtual void Activate()
	{
		_health = MaxHealth * DifficultyMultiplier;
		OnHealthChanged?.Invoke(_health);
		Vector2 randomDirection = new Vector2((float)GD.RandRange(-1, 1), (float)GD.RandRange(-1, 1)).Normalized();
		if (randomDirection == Vector2.Zero) randomDirection = Vector2.Right;
		float spawnDistance = (float)GD.RandRange(500, 800);
		GlobalPosition = Target.GlobalPosition + randomDirection * spawnDistance;
		Poolable.Activate(this);
	}

	public virtual void Deactivate()
	{
		Poolable.Deactivate(this);
	}
}