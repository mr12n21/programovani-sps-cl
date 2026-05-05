using Godot;

public partial class Minimap : Control
{
	public Node2D TrackedNode;
	[Export] public float MapScale = 0.05f;
	[Export] public Vector2 MapSize = new Vector2(200, 200);
	[Export] public Color BackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.7f);
	[Export] public Color PlayerColor = Colors.Green;
	[Export] public Color EnemyColor = Colors.Red;
	[Export] public Color BorderColor = Colors.White;

	public override void _Ready()
	{
		CustomMinimumSize = MapSize;
		Size = MapSize;
		CallDeferred(nameof(FindPlayer));
	}

	private void FindPlayer()
	{
		var game = GetTree().Root.GetNodeOrNull<Game>("Game");
		if (game != null)
			TrackedNode = game.GetNodeOrNull<Player>("Player");
	}

	public override void _Process(double delta)

	{
		QueueRedraw();
	}

	public override void _Draw()
	{
		DrawRect(new Rect2(Vector2.Zero, MapSize), BackgroundColor);
		DrawRect(new Rect2(Vector2.Zero, MapSize), BorderColor, false, 2);

		if (TrackedNode == null) return;

		Vector2 center = MapSize / 2;
		Vector2 playerWorldPos = TrackedNode.GlobalPosition;

		DrawCircle(center, 4, PlayerColor);

		var tree = GetTree();
		if (tree == null) return;

		foreach (var node in tree.GetNodesInGroup("enemies"))
		{
			if (node is not Node2D enemy2D) continue;
			if (!enemy2D.Visible) continue;

			Vector2 relativePos = (enemy2D.GlobalPosition - playerWorldPos) * MapScale;
			Vector2 dotPos = center + relativePos;

			if (dotPos.X < 0 || dotPos.X > MapSize.X || dotPos.Y < 0 || dotPos.Y > MapSize.Y)
				continue;

			DrawCircle(dotPos, 3, EnemyColor);
		}
	}
}
