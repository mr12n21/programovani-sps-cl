using Godot;

public partial class MeleeEnemy : Enemy
{
	public override void _Ready()
	{
		base._Ready();
		Speed = 70;
		ScoreValue = 75;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Target == null) return;
		float distance = Target.GlobalPosition.DistanceTo(GlobalPosition);
		float rushMultiplier = distance < 200 ? 1.5f : 1.0f;

		Head.Rotation = (Target.GlobalPosition - GlobalPosition).Angle() - Mathf.Pi / 2;
		Agent.SetTargetPosition(Target.GlobalPosition);
		Vector2 velocity = (Agent.GetNextPathPosition() - GlobalPosition).Normalized()
			* Speed * DifficultyMultiplier * rushMultiplier;
		Velocity = velocity;
		MoveAndSlide();
	}

	public override void Deactivate()
	{
		Poolable.Deactivate(this);
	}
}