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
	public bool IsActive { get; private set; }
	public bool IsBoss { get; private set; }
	public int DiamondReward { get; private set; } = 1;

	private uint _collisionLayer = 0;
	private float _baseSpeed;
	private float _baseMaxHealth;
	private Vector2 _baseScale;
	private Color _baseModulate;
	private Vector2? _spawnPosition;

	public override void _Ready()
	{
		_collisionLayer = CollisionLayer;
		_baseSpeed = Speed;
		_baseMaxHealth = MaxHealth;
		_baseScale = Scale;
		_baseModulate = Modulate;
		AddToGroup("enemies");
	}

	public void Die()
	{
		if (!IsActive)
		{
			return;
		}

		Game.Instance?.HandleEnemyDefeated(this);
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

	public virtual void ConfigureSpawn(Player target, int intensity, bool isBoss)
	{
		Target = target;
		IsBoss = isBoss;

		int safeIntensity = Math.Max(0, intensity);
		float healthMultiplier = 1f + safeIntensity * 0.24f + (isBoss ? 2.8f : 0f);
		float speedMultiplier = 1f + safeIntensity * 0.055f + (isBoss ? 0.25f : 0f);

		MaxHealth = _baseMaxHealth * healthMultiplier;
		Speed = _baseSpeed * speedMultiplier;
		Scale = _baseScale * (isBoss ? 1.65f + safeIntensity * 0.02f : 1f + safeIntensity * 0.035f);
		Modulate = isBoss ? new Color(0.76f, 0.9f, 1f, 1f) : _baseModulate;
		DiamondReward = isBoss ? 10 + safeIntensity * 2 : 1 + safeIntensity / 3;
		Health = MaxHealth;
	}

	public void SetSpawnPosition(Vector2 position)
	{
		_spawnPosition = position;
	}

	[Export] public Node2D EnemyHead;
	public override void _PhysicsProcess(double delta)
	{
		if (!IsActive || Target == null || Agent == null || EnemyHead == null)
		{
			return;
		}

		if (!IsBoss && Game.Instance != null && Game.Instance.ShouldRecycleEnemy(GlobalPosition, Target.GlobalPosition))
		{
			Deactivate();
			return;
		}

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
		if (Target == null)
		{
			return;
		}

		Health = MaxHealth;
		GlobalPosition = _spawnPosition ?? Target.GlobalPosition;
		IsActive = true;
		Poolable.Activate(this);
		Game.Instance?.RegisterEnemySpawn(this);
	}

	public virtual void Deactivate()
	{
		if (!IsActive)
		{
			return;
		}

		IsActive = false;
		Game.Instance?.RegisterEnemyReturned(this);
		ResetSpawnState();
		Poolable.Deactivate(this);
	}

	private void ResetSpawnState()
	{
		IsBoss = false;
		DiamondReward = 1;
		Speed = _baseSpeed;
		MaxHealth = _baseMaxHealth;
		Scale = _baseScale;
		Modulate = _baseModulate;
		Target = null;
		_spawnPosition = null;
		Health = MaxHealth;
	}
}
