using Godot;
using System;

public partial class RangeEnemy : Enemy
{
	[Export] Gun gun;
	private float _baseFireRate;

	public override void _Ready()
	{
		base._Ready();
		_baseFireRate = gun != null ? gun.FireRate : 1f;
	}

	public override void ConfigureSpawn(Player target, int intensity, bool isBoss)
	{
		base.ConfigureSpawn(target, intensity, isBoss);
		if (gun != null)
		{
			gun.FireRate = _baseFireRate * (1f + intensity * 0.05f + (isBoss ? 0.25f : 0f));
		}
	}

    public override void _PhysicsProcess(double delta)
    {
		if (!IsActive || Target == null || EnemyHead == null)
		{
			return;
		}

		if (Game.Instance != null && Game.Instance.ShouldRecycleEnemy(GlobalPosition, Target.GlobalPosition))
		{
			Deactivate();
			return;
		}

		if (Target.GlobalPosition.DistanceTo(GlobalPosition) > 300)
		{
			base._PhysicsProcess(delta);
		} else
		{
			EnemyHead.Rotation = (Target.GlobalPosition - GlobalPosition).Angle() - Mathf.Pi / 2;
			gun?.Shoot();
		}
  	}

    public override void Deactivate()
    {
		if (gun != null)
		{
			gun.FireRate = _baseFireRate;
		}
        base.Deactivate();
	}
}
