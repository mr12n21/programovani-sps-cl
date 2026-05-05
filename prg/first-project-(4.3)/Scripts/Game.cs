using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node2D
{
	public static Game Instance { get; private set; }

	public int Score
	{
		get
		{
			if (ScoreLabel != null && int.TryParse(ScoreLabel.Text, out int score)) return score;
			return 0;
		}
		set
		{
			if (ScoreLabel != null)
			{
				ScoreLabel.Text = value.ToString();
			}
		}
	}

	[Export]
	Godot.Collections.Dictionary<string, PoolObject> PoolObjects = new();
	[Export] public Label ScoreLabel;
	[Export] public float BaseSpawnInterval = 1.7f;
	[Export] public float MinimumSpawnInterval = 0.55f;
	[Export] public int BaseTargetEnemies = 4;
	[Export] public int MaxTargetEnemies = 18;
	[Export] public float BossSpawnEverySeconds = 40f;
	[Export] public int BossSpawnEveryKills = 18;

	private readonly Dictionary<string, Queue<Node>> _poolNodes = new();
	private readonly Dictionary<Type, string> _objectnames = new();
	private PackedScene _diamondPickupScene;
	private Timer _spawnTimer;
	private Player _player;
	private WeaponInventory _inventory;
	private WeaponHud _weaponHud;
	private int _activeEnemies = 0;
	private int _kills = 0;
	private int _bossesSpawned = 0;
	private bool _bossAlive = false;
	private float _runTime = 0f;
	
	public override void _Ready()
	{
		foreach (var keyValuePair in PoolObjects)
		{
			PoolObject poolObject = keyValuePair.Value;
			string name = keyValuePair.Key;
			Queue<Node> nodes = new();
			_poolNodes[name] = nodes;
			for (int i = 0; i < poolObject.PoolSize; i++)
			{
				Node node = poolObject.Prefab.Instantiate();
				if (node is not IPoolable poolable) break;
				_objectnames[node.GetType()] = name;
				nodes.Enqueue(node);
				AddChild(node);
				poolable.Init();
				
			}
		}
		Instance = this;
		_player = GetNodeOrNull<Player>("Player");
		_inventory = GetNodeOrNull<WeaponInventory>("WeaponInventory");
		_weaponHud = GetNodeOrNull<CanvasLayer>("CanvasLayer")?.GetNodeOrNull<WeaponHud>("WeaponHud");
		_spawnTimer = GetNodeOrNull<Timer>("Timer");
		if (_spawnTimer != null)
		{
			_spawnTimer.WaitTime = BaseSpawnInterval;
			_spawnTimer.OneShot = false;
			if (_spawnTimer.IsStopped())
			{
				_spawnTimer.Start();
			}
		}

		_diamondPickupScene = GD.Load<PackedScene>("res://Scenes/DiamondPickup.tscn");
		UpdateDirectorStatus();
		WarmupSpawn();
	}

	public Node GetPoolObject(string name)
	{
		if (!_poolNodes.ContainsKey(name)) return null;
		Queue<Node> nodes = _poolNodes[name];
		if (nodes.Count <= 0) return null;
		return nodes.Dequeue();
	}

	public T GetPoolObject<T>() where T : Node
	{
		if (!_objectnames.ContainsKey(typeof(T))) return null;
		return (T)GetPoolObject(_objectnames[typeof(T)]);
	}

	public void EnqueuePoolObject(string name, Node node)
	{
		if (!_poolNodes.ContainsKey(name)) return;
		Queue<Node> nodes = _poolNodes[name];
		nodes.Enqueue(node);
	}

	public void EnqueuePoolObject<T>(T node) where T : Node
	{
		if (!_objectnames.ContainsKey(typeof(T))) return;
		EnqueuePoolObject(_objectnames[typeof(T)], node);
	}

	public void SpawnEnemy()
	{
		EnsureReferences();
		if (_player == null)
		{
			return;
		}

		int targetEnemies = GetTargetEnemyCount();
		int missingEnemies = Mathf.Max(0, targetEnemies - _activeEnemies);
		int attempts = Mathf.Clamp(missingEnemies == 0 ? 1 : missingEnemies, 1, 3);

		for (int i = 0; i < attempts; i++)
		{
			if (ShouldSpawnBoss() && SpawnConfiguredEnemy(true))
			{
				break;
			}

			if (_activeEnemies >= targetEnemies)
			{
				break;
			}

			SpawnConfiguredEnemy(false);
		}
	}

	public override void _Process(double delta)
	{
		_runTime += (float)delta;

		if (_spawnTimer != null)
		{
			_spawnTimer.WaitTime = GetCurrentSpawnInterval();
		}

		UpdateDirectorStatus();
	}

	public void RegisterEnemySpawn(Enemy enemy)
	{
		_activeEnemies += 1;
		if (enemy.IsBoss)
		{
			_bossAlive = true;
		}
		UpdateDirectorStatus();
	}

	public void RegisterEnemyReturned(Enemy enemy)
	{
		_activeEnemies = Mathf.Max(0, _activeEnemies - 1);
		UpdateDirectorStatus();
	}

	public void HandleEnemyDefeated(Enemy enemy)
	{
		_kills += 1;
		Score += enemy.IsBoss ? 5 : 1;

		if (enemy.IsBoss)
		{
			_bossAlive = false;
		}

		SpawnDiamondBurst(enemy.GlobalPosition, enemy.DiamondReward);
		UpdateDirectorStatus();
	}

	private void WarmupSpawn()
	{
		for (int i = 0; i < Mathf.Min(3, BaseTargetEnemies); i++)
		{
			SpawnConfiguredEnemy(false);
		}
	}

	private bool SpawnConfiguredEnemy(bool boss)
	{
		EnsureReferences();
		Enemy enemy = RequestEnemyFromPool();
		if (enemy == null || _player == null)
		{
			return false;
		}

		int intensity = GetDifficultyTier() + (boss ? 2 : 0);
		enemy.ConfigureSpawn(_player, intensity, boss);
		enemy.Activate();

		if (boss)
		{
			_bossAlive = true;
			_bossesSpawned += 1;
		}

		return true;
	}

	private Enemy RequestEnemyFromPool()
	{
		int randomEnemy = GD.RandiRange(0, 1);
		Enemy enemy;
		if (randomEnemy == 0)
		{
			enemy = GetPoolObject<MeleeEnemy>();
			if (enemy == null) enemy = GetPoolObject<RangeEnemy>();
		}
		else
		{
			enemy = GetPoolObject<RangeEnemy>();
			if (enemy == null) enemy = GetPoolObject<MeleeEnemy>();
		}

		return enemy;
	}

	private int GetDifficultyTier()
	{
		return 1 + (int)(_runTime / 35f) + (_kills / 12) + _bossesSpawned;
	}

	private int GetTargetEnemyCount()
	{
		int targetEnemies = BaseTargetEnemies + (int)(_runTime / 24f) + (_kills / 8);
		return Mathf.Clamp(targetEnemies, BaseTargetEnemies, MaxTargetEnemies);
	}

	private float GetCurrentSpawnInterval()
	{
		float spawnInterval = BaseSpawnInterval - _runTime * 0.015f - _kills * 0.01f;
		return Mathf.Max(MinimumSpawnInterval, spawnInterval);
	}

	private bool ShouldSpawnBoss()
	{
		if (_bossAlive)
		{
			return false;
		}

		int nextBossTier = _bossesSpawned + 1;
		return _runTime >= nextBossTier * BossSpawnEverySeconds || _kills >= nextBossTier * BossSpawnEveryKills;
	}

	private void SpawnDiamondBurst(Vector2 position, int reward)
	{
		if (_diamondPickupScene == null || reward <= 0)
		{
			return;
		}

		int count = Mathf.Clamp(reward / 2 + 1, 1, 6);
		int remaining = reward;
		for (int i = 0; i < count; i++)
		{
			var pickup = _diamondPickupScene.Instantiate<DiamondPickup>();
			if (pickup == null)
			{
				return;
			}

			int piecesLeft = count - i;
			int value = Mathf.Max(1, Mathf.CeilToInt((float)remaining / piecesLeft));
			remaining -= value;

			AddChild(pickup);
			pickup.GlobalPosition = position + RandomOffset(16f, 58f);
			pickup.SetValue(value);
		}
	}

	private Vector2 RandomOffset(float minRadius, float maxRadius)
	{
		Vector2 direction = new Vector2(GD.RandfRange(-1f, 1f), GD.RandfRange(-1f, 1f)).Normalized();
		if (direction == Vector2.Zero)
		{
			direction = Vector2.Right;
		}

		return direction * GD.RandfRange(minRadius, maxRadius);
	}

	private void EnsureReferences()
	{
		if (_player == null)
		{
			_player = GetNodeOrNull<Player>("Player");
		}

		if (_inventory == null)
		{
			_inventory = GetNodeOrNull<WeaponInventory>("WeaponInventory");
		}

		if (_weaponHud == null)
		{
			_weaponHud = GetNodeOrNull<CanvasLayer>("CanvasLayer")?.GetNodeOrNull<WeaponHud>("WeaponHud");
		}
	}

	private void UpdateDirectorStatus()
	{
		EnsureReferences();
		if (_weaponHud == null)
		{
			return;
		}

		string status = _bossAlive
			? $"BOSS WAVE  LVL {GetDifficultyTier()}"
			: $"WAVE {GetDifficultyTier()}  {_activeEnemies}/{GetTargetEnemyCount()}";
		_weaponHud.SetDirectorStatus(status);
	}
}
