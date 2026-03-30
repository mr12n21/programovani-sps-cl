using Godot;
using System;

public partial class ShootingEnemy : Enemy
{
	[Export] Gun Gun;
	private float _strafeTimer;

	public override void _Ready()
	{
		base._Ready();
		ScoreValue = 150;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Target == null) return;
		float distance = Target.GlobalPosition.DistanceTo(GlobalPosition);

		if (distance > 350)
		{
			base._PhysicsProcess(delta);
		}
		else if (distance < 200)
		{
			// Too close - retreat while shooting
			Head.Rotation = (Target.GlobalPosition - GlobalPosition).Angle() - Mathf.Pi / 2;
			Vector2 awayDir = (GlobalPosition - Target.GlobalPosition).Normalized();
			Velocity = awayDir * Speed * DifficultyMultiplier * 0.7f;
			MoveAndSlide();
			Gun.Shoot();
		}
		else
		{
			// Strafe and shoot
			_strafeTimer += (float)delta;
			Head.Rotation = (Target.GlobalPosition - GlobalPosition).Angle() - Mathf.Pi / 2;
			Vector2 toTarget = (Target.GlobalPosition - GlobalPosition).Normalized();
			Vector2 strafeDir = toTarget.Rotated(Mathf.Pi / 2 * Mathf.Sign(Mathf.Sin(_strafeTimer * 2)));
			Velocity = strafeDir * Speed * DifficultyMultiplier * 0.5f;
			MoveAndSlide();
			Gun.Shoot();
		}
	}

	public override void Deactivate()
	{
		_strafeTimer = 0;
		Poolable.Deactivate(this);
	}
}
