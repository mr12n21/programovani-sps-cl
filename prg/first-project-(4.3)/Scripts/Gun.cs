using Godot;
using System;

public partial class Gun : Node2D
{
	float FireTime = 1000;
	[Export] public  float FireRate
	{
		get => 1000/FireTime;
		set => FireTime = 1000/value;
	}
	[Export] public Node2D ProjectileSpawn;
	[Export] public uint ProjectileMask = 0;
	double _lastFire = 0;

	private float _damageOverride = -1;
	private Sprite2D _sprite;

	public override void _Ready()
	{
		if (ProjectileSpawn == null) {
			ProjectileSpawn = GetNode<Node2D>("ProjectileSpawn");
		}
		_sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
	}

	public void ApplyWeapon(WeaponData weapon)
	{
		if (weapon == null) return;
		FireRate = weapon.FireRate;
		_damageOverride = weapon.Damage;
		if (_sprite != null && !string.IsNullOrEmpty(weapon.IconPath))
		{
			var tex = GD.Load<Texture2D>(weapon.IconPath);
			if (tex != null) _sprite.Texture = tex;
		}
	}

	public override void _Process(double delta)
	{
	}

	public void Shoot()
	{
		if (Time.GetTicksMsec() - _lastFire < FireTime) return;
		_lastFire = Time.GetTicksMsec();
		Projectile projectile = Game.Instance.GetPoolObject<Projectile>();
		if (projectile == null) return;
		var direction = ProjectileSpawn.GlobalPosition - GlobalPosition;
		projectile.CollisionMask = ProjectileMask;
		projectile.GlobalPosition = GlobalPosition;
		projectile.Rotation = direction.Angle();
		if (_damageOverride > 0) projectile.Damage = _damageOverride;
		projectile.Activate();
	}
}
