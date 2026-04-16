using Godot;
using System;

public partial class Player : CharacterBody2D, IDamageable
{
	public const float Speed = 300.0f;
	public const float SprintSpeed = 550.0f;
	public const float MaxNitro = 100f;
	public const float NitroDrainRate = 40f;   // per second
	public const float NitroRegenRate = 20f;    // per second
	[Export] public Node2D PlayerHead;
	[Export] Gun gun;

	public float Nitro { get; private set; } = MaxNitro;
	public bool IsSprinting { get; private set; } = false;
	public event Action<float> OnNitroChanged;

	public WeaponInventory Inventory;

	public override void _Ready()
	{
		_collisionLayer = CollisionLayer;
		ProcessMode = ProcessModeEnum.Always;
		CallDeferred(nameof(FindInventory));
	}

	private void FindInventory()
	{
		var parent = GetParent();
		if (parent != null)
		{
			Inventory = parent.GetNodeOrNull<WeaponInventory>("WeaponInventory");
		}
		if (Inventory != null)
		{
			Inventory.OnWeaponChanged += OnWeaponChanged;
			ApplyInitialWeapon();
		}
	}

	private void ApplyInitialWeapon()
	{
		if (Inventory?.CurrentWeapon != null)
			gun.ApplyWeapon(Inventory.CurrentWeapon);
	}

	private void OnWeaponChanged(WeaponData weapon, int index)
	{
		gun.ApplyWeapon(weapon);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (GetTree().Paused) return;
		if (PlayerImage != null) {
			PlayerHead.Rotation = (GetGlobalMousePosition() - GlobalPosition).Angle() - Mathf.Pi / 2;
		}
		Vector2 vector2 = Input.GetVector("Left", "Right", "Up", "Down");
		vector2 = vector2.Normalized();

		// Sprint / nitro
		bool wantsToSprint = Input.IsKeyPressed(Key.Shift) && vector2.Length() > 0.1f;
		if (wantsToSprint && Nitro > 0f)
		{
			IsSprinting = true;
			Nitro = Mathf.Max(0f, Nitro - NitroDrainRate * (float)delta);
		}
		else
		{
			IsSprinting = false;
			Nitro = Mathf.Min(MaxNitro, Nitro + NitroRegenRate * (float)delta);
		}
		OnNitroChanged?.Invoke(Nitro);

		float currentSpeed = IsSprinting ? SprintSpeed : Speed;

		Vector2 velocity = Velocity;
		velocity.X = Mathf.Lerp(velocity.X, vector2.X * currentSpeed, 0.1f);
		velocity.Y = Mathf.Lerp(velocity.Y, vector2.Y * currentSpeed, 0.1f);
		Velocity = velocity;
		MoveAndSlide();
	}
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton MouseButtonEvent)
		{
			if(MouseButtonEvent.ButtonIndex == MouseButton.Left && MouseButtonEvent.Pressed && !_inventoryOpen)
			{
				gun.Shoot();
			}
			if(MouseButtonEvent.ButtonIndex == MouseButton.WheelUp && MouseButtonEvent.Pressed)
			{
				Inventory?.Next();
			}
			if(MouseButtonEvent.ButtonIndex == MouseButton.WheelDown && MouseButtonEvent.Pressed)
			{
				Inventory?.Previous();
			}
		}
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (keyEvent.Keycode >= Key.Key1 && keyEvent.Keycode <= Key.Key5)
			{
				int index = (int)keyEvent.Keycode - (int)Key.Key1;
				Inventory?.SwitchTo(index);
			}
			if (keyEvent.Keycode == Key.Tab)
			{
				ToggleInventory();
			}
		}
    }
	private bool _inventoryOpen = false;

	private void ToggleInventory()
	{
		_inventoryOpen = !_inventoryOpen;
		GetTree().Paused = _inventoryOpen;
		var hud = GetTree().Root.GetNodeOrNull<Game>("Game")
			?.GetNodeOrNull<CanvasLayer>("CanvasLayer")
			?.GetNodeOrNull<WeaponHud>("WeaponHud");
		if (hud != null) hud.SetInventoryOpen(_inventoryOpen);
	}

[Export]
	public float MaxHealth { get; set; } = 100;
	[Export] public Sprite2D PlayerImage;
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

	private uint _collisionLayer = 1;

    public void Heal(float hp)
    {
        Health += hp;
    }

    public void TakeDamage(float damage)
    {
		Health -= damage;
    }

	public void Die()
	{
		GD.PushError("!!! YOU DIED !!! YOU GOT SLIMED BY THE 🧃 !!!");
		Game.Instance.GetTree().Quit();
	}

}
