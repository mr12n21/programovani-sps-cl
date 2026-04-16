using Godot;
using System;

public partial class RangeEnemy : Enemy
{
	[Export] Gun gun;

    public override void _PhysicsProcess(double delta)
    {
		if (Target.GlobalPosition.DistanceTo(GlobalPosition) > 300)
		{
			base._PhysicsProcess(delta);
		} else
		{
			EnemyHead.Rotation = (Target.GlobalPosition - GlobalPosition).Angle() - Mathf.Pi / 2;
			gun.Shoot();
		}
  	}

    public override void Deactivate()
    {
        base.Deactivate();
	}
}
