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
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (ProjectileSpawn == null) {
			ProjectileSpawn = GetNode<Node2D>("ProjectileSpawn");
		}
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
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
		projectile.Activate();
	}
}
