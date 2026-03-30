using Godot;
using System;

public partial class Gun : Node2D
{
	[Export] public uint ProjectileMask = 0;
	[Export] Node2D ProjectileSpawn;
	[Export] public float FireRate 
	{
		get => 1000 / FireTime;
		set => FireTime = 1000 / value;
	}
	float FireTime = 1000;
	double _lastFireTime;
	public override void _Ready()
	{
		if (ProjectileSpawn == null)
			ProjectileSpawn = GetNode<Node2D>("ProjectileSpawn");
	}

	public void Shoot()
	{
		if (Time.GetTicksMsec() - _lastFireTime < FireTime) return;
		_lastFireTime = Time.GetTicksMsec();
		Projectile projectile = Game.Instance.GetPoolObject<Projectile>();
		if (projectile == null) return;
		var direction = ProjectileSpawn.GlobalPosition - GlobalPosition;
		projectile.CollisionMask = ProjectileMask;
		projectile.GlobalPosition = GlobalPosition;
		projectile.Rotation = direction.Angle();
		projectile.Activate();
	}
}
