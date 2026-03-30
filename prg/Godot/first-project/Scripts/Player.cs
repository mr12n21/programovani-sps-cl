using Godot;
using System;

public partial class Player : CharacterBody2D, IDamageable
{
	public const float Speed = 300.0f;
	private const float DashSpeed = 800.0f;
	private const float DashDuration = 0.15f;
	private const float DashCooldown = 1.0f;
	private const float InvincibilityDuration = 0.5f;

	[Export] public float MaxHealth { get; set; } = 100;
	[Export] public Sprite2D PlayerImage;

	public float Health
	{
		get => _health;
		set
		{
			value = Math.Clamp(value, 0, MaxHealth);
			if (Math.Abs(value - _health) < 0.001f) return;
			_health = value;
			OnHealthChanged?.Invoke(_health);
			if (_health <= 0) Die();
		}
	}

	private float _health;
	public event Action<float> OnHealthChanged;

	private bool _isDashing;
	private float _dashTimer;
	private float _dashCooldownTimer;
	private Vector2 _dashDirection;
	private bool _isDead;
	private float _invincibilityTimer;

	public override void _Ready()
	{
		_health = MaxHealth;
		_isDead = false;
		OnHealthChanged?.Invoke(_health);
	}

	public void Die()
	{
		if (_isDead) return;
		_isDead = true;
		ProcessMode = ProcessModeEnum.Disabled;
		Game.Instance?.OnPlayerDied();
	}

	public void Heal(float amount) => Health += amount;

	public void TakeDamage(float damage)
	{
		if (_isDead || _invincibilityTimer > 0 || _isDashing) return;
		Health -= damage;
		_invincibilityTimer = InvincibilityDuration;
		if (PlayerImage != null)
			PlayerImage.Modulate = new Color(1, 0.3f, 0.3f);
		Game.Instance?.ShakeCamera(5f, 0.2f);
	}

	public override void _Input(InputEvent @event)
	{
		if (_isDead) return;
		if (@event is InputEventMouseButton mouseButtonEvent)
		{
			if (mouseButtonEvent.ButtonIndex == MouseButton.Left && mouseButtonEvent.Pressed)
			{
				Shoot(mouseButtonEvent.Position - GlobalPosition);
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isDead) return;
		float dt = (float)delta;

		// Invincibility flash
		if (_invincibilityTimer > 0)
		{
			_invincibilityTimer -= dt;
			if (PlayerImage != null)
			{
				float t = 1 - _invincibilityTimer / InvincibilityDuration;
				PlayerImage.Modulate = _invincibilityTimer > 0
					? new Color(1, 0.3f + 0.7f * t, 0.3f + 0.7f * t)
					: Colors.White;
			}
		}

		// Rotate to face mouse
		if (PlayerImage != null)
			PlayerImage.Rotation = (GetGlobalMousePosition() - GlobalPosition).Angle() - Mathf.Pi / 2;

		// Dash cooldown
		if (_dashCooldownTimer > 0) _dashCooldownTimer -= dt;

		if (_isDashing)
		{
			_dashTimer -= dt;
			if (_dashTimer <= 0) _isDashing = false;
			Velocity = _dashDirection * DashSpeed;
			MoveAndSlide();
			return;
		}

		// Start dash (Shift key)
		if (Input.IsKeyPressed(Key.Shift) && _dashCooldownTimer <= 0)
		{
			Vector2 dir = Input.GetVector("left", "right", "up", "down").Normalized();
			if (dir != Vector2.Zero)
			{
				_isDashing = true;
				_dashTimer = DashDuration;
				_dashCooldownTimer = DashCooldown;
				_dashDirection = dir;
			}
		}

		// Normal movement
		Vector2 vector2 = Input.GetVector("left", "right", "up", "down").Normalized();
		Vector2 velocity = Velocity;
		velocity.X = Mathf.Lerp(Velocity.X, vector2.X * Speed, 0.1f);
		velocity.Y = Mathf.Lerp(Velocity.Y, vector2.Y * Speed, 0.1f);
		Velocity = velocity;
		MoveAndSlide();
	}

	private void Shoot(Vector2 direction)
	{
		Projectile projectile = Game.Instance?.GetPoolObject<Projectile>();
		if (projectile == null) return;
		projectile.Damage = 25;
		projectile.CollisionMask = 1 << 1;
		projectile.GlobalPosition = GlobalPosition;
		projectile.Rotation = direction.Angle();
		projectile.Activate();
	}

	public void Reset()
	{
		_isDead = false;
		_health = MaxHealth;
		_invincibilityTimer = 0;
		_isDashing = false;
		GlobalPosition = Vector2.Zero;
		ProcessMode = ProcessModeEnum.Inherit;
		Velocity = Vector2.Zero;
		if (PlayerImage != null) PlayerImage.Modulate = Colors.White;
		OnHealthChanged?.Invoke(_health);
	}
}
