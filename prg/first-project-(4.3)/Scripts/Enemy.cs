using Godot;
using System;

public abstract partial class Enemy : CharacterBody2D, IDamageable, IPoolable
{
	[Export] public Node2D Target;
	[Export] protected NavigationAgent2D Agent;
	[Export] protected float Speed = 50;

	[Export]
	public float MaxHealth { get; set; } = 100;
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

	private uint _collisionLayer = 0;

	public override void _Ready()
	{
		_collisionLayer = CollisionLayer;
		AddToGroup("enemies");
	}

	public void Die()
	{
		Game.Instance.Score+= 1;
		Deactivate();
	}

	public void Heal(float hp)
	{
		Health += hp;
	}

	public void TakeDamage(float damage)
	{
		Health -= damage;
	}

	[Export] public Node2D EnemyHead;
	public override void _PhysicsProcess(double delta)
	{
		EnemyHead.Rotation = (Target.GlobalPosition - GlobalPosition).Angle() - Mathf.Pi / 2;
		Agent.SetTargetPosition(Target.GlobalPosition);
		Vector2 velocity = (Agent.GetNextPathPosition() - GlobalPosition).Normalized() * Speed;
		Velocity = velocity;
		MoveAndSlide();
	}

	public virtual void Init()
	{
		Poolable.Init(this);
	}

	public virtual void Activate()
	{
		Health = MaxHealth;
		Vector2 RandomDirection = new Vector2(GD.RandRange(-1, 1), GD.RandRange(-1, 1)).Normalized();
		if (RandomDirection.X == 0 && RandomDirection.Y == 0) RandomDirection = Vector2.Right;
		Vector2 RandomPosition = RandomDirection * 500;
		GlobalPosition = Target.GlobalPosition + RandomPosition;
		Poolable.Activate(this);
	}

	public virtual void Deactivate()
	{
		Poolable.Deactivate(this);
	}
}
