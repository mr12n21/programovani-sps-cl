using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node2D
{
	public static Game Instance { get; private set; }

	[Export]
	Godot.Collections.Dictionary<string, PoolObject> PoolObjects = new();

	private Dictionary<string, Queue<Node>> _poolNodes = new();
	private Dictionary<Type, string> _objectNames = new();

	// Game state
	public int Score { get; private set; }
	public int Wave { get; private set; }
	public int KillCount { get; private set; }
	public bool IsGameOver { get; private set; }

	// Wave system
	private int _enemiesRemainingInWave;
	private int _enemiesAliveInWave;
	private float _spawnTimer;
	private float _spawnInterval = 2.0f;
	private float _waveBreakTimer;
	private bool _inWaveBreak;

	// Camera shake
	private Camera2D _camera;
	private float _shakeIntensity;
	private float _shakeDuration;
	private float _shakeTimer;

	// HUD
	private HUD _hud;

	// Events
	public event Action<int> OnScoreChanged;
	public event Action<int> OnWaveChanged;
	public event Action<int> OnKillCountChanged;

	public override void _Ready()
	{
		Instance = this;

		// Initialize pool
		foreach (var keyValuePair in PoolObjects)
		{
			PoolObject poolObject = keyValuePair.Value;
			string name = keyValuePair.Key;
			Queue<Node> nodes = new Queue<Node>();
			_poolNodes[name] = nodes;
			for (int i = 0; i < poolObject.PoolSize; i++)
			{
				Node node = poolObject.Prefab.Instantiate();
				if (node is not IPoolable poolable) break;
				_objectNames[node.GetType()] = name;
				nodes.Enqueue(node);
				AddChild(node);
				poolable.Init();
			}
		}

		CreateBackground();
		CreateObstacles();
		SetupCamera();

		_hud = new HUD();
		AddChild(_hud);

		StartGame();
	}

	public override void _ExitTree()
	{
		if (Instance == this)
			Instance = null;
	}

	private void StartGame()
	{
		Score = 0;
		Wave = 0;
		KillCount = 0;
		IsGameOver = false;
		_inWaveBreak = true;
		_waveBreakTimer = 2.0f;

		OnScoreChanged?.Invoke(Score);
		OnWaveChanged?.Invoke(Wave);
		OnKillCountChanged?.Invoke(KillCount);
	}

	public override void _Process(double delta)
	{
		float dt = (float)delta;

		// Camera shake
		if (_shakeTimer > 0 && _camera != null)
		{
			_shakeTimer -= dt;
			float intensity = _shakeIntensity * (_shakeTimer / _shakeDuration);
			_camera.Offset = new Vector2(
				(float)GD.RandRange(-intensity, intensity),
				(float)GD.RandRange(-intensity, intensity)
			);
			if (_shakeTimer <= 0)
				_camera.Offset = Vector2.Zero;
		}

		if (IsGameOver) return;

		// Wave break
		if (_inWaveBreak)
		{
			_waveBreakTimer -= dt;
			if (_waveBreakTimer <= 0)
				StartNextWave();
			return;
		}

		// Spawn enemies
		if (_enemiesRemainingInWave > 0)
		{
			_spawnTimer -= dt;
			if (_spawnTimer <= 0)
			{
				SpawnEnemy();
				_enemiesRemainingInWave--;
				_spawnTimer = _spawnInterval;
			}
		}

		// Check wave complete
		if (_enemiesRemainingInWave <= 0 && _enemiesAliveInWave <= 0)
		{
			_inWaveBreak = true;
			_waveBreakTimer = 3.0f;
		}
	}

	private void StartNextWave()
	{
		Wave++;
		_inWaveBreak = false;
		_enemiesRemainingInWave = 3 + Wave * 2;
		_enemiesAliveInWave = 0;
		_spawnInterval = Math.Max(0.5f, 2.0f - Wave * 0.1f);
		_spawnTimer = 0.5f;
		OnWaveChanged?.Invoke(Wave);
	}

	public void ShakeCamera(float intensity, float duration)
	{
		_shakeIntensity = intensity;
		_shakeDuration = duration;
		_shakeTimer = duration;
	}

	private void SetupCamera()
	{
		Player player = GetNodeOrNull<Player>("Player");
		if (player == null) return;

		_camera = new Camera2D();
		_camera.PositionSmoothingEnabled = true;
		_camera.PositionSmoothingSpeed = 8;
		_camera.Zoom = new Vector2(0.8f, 0.8f);
		player.AddChild(_camera);
		_camera.MakeCurrent();
	}

	private void CreateBackground()
	{
		// Dark background
		var bg = new ColorRect();
		bg.Color = new Color(0.08f, 0.08f, 0.15f);
		bg.Position = new Vector2(-2000, -2000);
		bg.Size = new Vector2(4000, 4000);
		bg.ZIndex = -10;
		AddChild(bg);
		MoveChild(bg, 0);

		// Grid lines
		for (int i = -2000; i <= 2000; i += 200)
		{
			var hLine = new Line2D();
			hLine.AddPoint(new Vector2(-2000, i));
			hLine.AddPoint(new Vector2(2000, i));
			hLine.DefaultColor = new Color(0.15f, 0.15f, 0.25f, 0.5f);
			hLine.Width = 1;
			hLine.ZIndex = -9;
			AddChild(hLine);

			var vLine = new Line2D();
			vLine.AddPoint(new Vector2(i, -2000));
			vLine.AddPoint(new Vector2(i, 2000));
			vLine.DefaultColor = new Color(0.15f, 0.15f, 0.25f, 0.5f);
			vLine.Width = 1;
			vLine.ZIndex = -9;
			AddChild(vLine);
		}

		// Arena boundary
		float arenaSize = 1500;
		var boundary = new Line2D();
		boundary.AddPoint(new Vector2(-arenaSize, -arenaSize));
		boundary.AddPoint(new Vector2(arenaSize, -arenaSize));
		boundary.AddPoint(new Vector2(arenaSize, arenaSize));
		boundary.AddPoint(new Vector2(-arenaSize, arenaSize));
		boundary.AddPoint(new Vector2(-arenaSize, -arenaSize));
		boundary.DefaultColor = new Color(0.4f, 0.2f, 0.8f, 0.8f);
		boundary.Width = 3;
		boundary.ZIndex = -8;
		AddChild(boundary);
	}

	private void CreateObstacles()
	{
		Vector2[] positions = {
			new(300, 200), new(-400, 300), new(500, -300),
			new(-300, -400), new(0, 500), new(-600, 0),
			new(600, 100), new(200, -600), new(-200, 600),
			new(700, -500), new(-700, 500), new(400, 700),
		};
		Vector2[] sizes = {
			new(80, 80), new(120, 60), new(60, 120),
			new(100, 100), new(150, 50), new(50, 150),
			new(90, 90), new(70, 130), new(130, 70),
			new(80, 80), new(110, 80), new(80, 110),
		};

		for (int i = 0; i < positions.Length; i++)
			CreateObstacle(positions[i], sizes[i]);
	}

	private void CreateObstacle(Vector2 position, Vector2 size)
	{
		var obstacle = new StaticBody2D();
		obstacle.Position = position;
		obstacle.CollisionLayer = 3;
		obstacle.CollisionMask = 0;

		var shape = new CollisionShape2D();
		var rect = new RectangleShape2D();
		rect.Size = size;
		shape.Shape = rect;
		obstacle.AddChild(shape);

		var visual = new ColorRect();
		visual.Color = new Color(0.3f, 0.3f, 0.4f);
		visual.Size = size;
		visual.Position = -size / 2;
		obstacle.AddChild(visual);

		var border = new Line2D();
		border.AddPoint(new Vector2(-size.X / 2, -size.Y / 2));
		border.AddPoint(new Vector2(size.X / 2, -size.Y / 2));
		border.AddPoint(new Vector2(size.X / 2, size.Y / 2));
		border.AddPoint(new Vector2(-size.X / 2, size.Y / 2));
		border.AddPoint(new Vector2(-size.X / 2, -size.Y / 2));
		border.DefaultColor = new Color(0.5f, 0.5f, 0.7f);
		border.Width = 2;
		obstacle.AddChild(border);

		AddChild(obstacle);
	}

	// Pool methods
	public Node GetPoolObject(string name)
	{
		if (!_poolNodes.ContainsKey(name)) return null;
		Queue<Node> nodes = _poolNodes[name];
		if (nodes.Count > 0) return nodes.Dequeue();
		return null;
	}

	public T GetPoolObject<T>() where T : Node
	{
		if (!_objectNames.ContainsKey(typeof(T))) return null;
		return (T)GetPoolObject(_objectNames[typeof(T)]);
	}

	public void EnqueuePoolObject(string name, Node node)
	{
		if (!_poolNodes.ContainsKey(name)) return;
		Queue<Node> nodes = _poolNodes[name];
		nodes.Enqueue(node);
	}

	public void EnqueuePoolObject<T>(T node) where T : Node
	{
		if (!_objectNames.ContainsKey(typeof(T))) return;
		EnqueuePoolObject(_objectNames[typeof(T)], node);
	}

	public void SpawnEnemy()
	{
		Enemy enemy;
		bool preferShooting = Wave > 3 && GD.RandRange(0, 2) > 0;

		if (preferShooting)
		{
			enemy = GetPoolObject<ShootingEnemy>();
			enemy ??= GetPoolObject<MeleeEnemy>();
		}
		else
		{
			enemy = GetPoolObject<MeleeEnemy>();
			enemy ??= GetPoolObject<ShootingEnemy>();
		}

		if (enemy == null) return;

		enemy.DifficultyMultiplier = 1.0f + (Wave - 1) * 0.15f;
		enemy.Target = GetNode<Player>("Player");
		enemy.Activate();
		_enemiesAliveInWave++;
	}

	public void OnEnemyKilled(Enemy enemy)
	{
		_enemiesAliveInWave = Math.Max(0, _enemiesAliveInWave - 1);
		KillCount++;
		Score += (int)(enemy.ScoreValue * (1 + Wave * 0.1f));
		OnScoreChanged?.Invoke(Score);
		OnKillCountChanged?.Invoke(KillCount);
	}

	public void OnPlayerDied()
	{
		IsGameOver = true;
		_hud?.ShowGameOver();
	}

	public void RestartGame()
	{
		GetNode<Player>("Player")?.Reset();
		IsGameOver = false;
		StartGame();
		_hud?.HideGameOver();
	}
}
