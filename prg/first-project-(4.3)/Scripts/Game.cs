using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node2D
{
	public static Game Instance { get; private set; }

	[Export]
	Godot.Collections.Dictionary<string, PoolObject> PoolObjects = new();
	[Export] public float BaseSpawnInterval = 1.7f;
	[Export] public float MinimumSpawnInterval = 0.55f;
	[Export] public int BaseTargetEnemies = 4;
	[Export] public int MaxTargetEnemies = 18;
	[Export] public float BossSpawnEverySeconds = 40f;
	[Export] public int BossSpawnEveryKills = 18;
	[Export] public float DirectorRefillInterval = 0.45f;
	[Export] public float SpawnPadding = 220f;
	[Export] public float EnemyRecycleDistance = 1800f;

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
	private float _refillCooldown = 0f;
	private Rect2 _worldBounds = new Rect2();
	private int _enemyPoolCapacity = 0;
	
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
				if (node is Enemy)
				{
					_enemyPoolCapacity += 1;
				}
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
		CacheWorldBounds();
		ApplyScenePalette();
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
		_refillCooldown = Mathf.Max(0f, _refillCooldown - (float)delta);

		if (_spawnTimer != null)
		{
			_spawnTimer.WaitTime = GetCurrentSpawnInterval();
		}

		if (_refillCooldown <= 0f && _activeEnemies < GetTargetEnemyCount())
		{
			_refillCooldown = DirectorRefillInterval;
			SpawnEnemy();
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
		if (enemy.IsBoss)
		{
			_bossAlive = false;
		}
		UpdateDirectorStatus();
	}

	public Vector2 GetEnemySpawnPosition(Vector2 center, bool boss)
	{
		float spawnRadius = boss ? 760f : 500f;
		Vector2 fallback = center + RandomDirection() * spawnRadius;
		Rect2 spawnBounds = GetSpawnBounds();

		if (spawnBounds.Size == Vector2.Zero)
		{
			return fallback;
		}

		for (int i = 0; i < 12; i++)
		{
			Vector2 candidate = center + RandomDirection() * RandomFloat(spawnRadius * 0.75f, spawnRadius * 1.15f);
			candidate.X = Mathf.Clamp(candidate.X, spawnBounds.Position.X, spawnBounds.End.X);
			candidate.Y = Mathf.Clamp(candidate.Y, spawnBounds.Position.Y, spawnBounds.End.Y);

			if (candidate.DistanceTo(center) >= spawnRadius * 0.55f)
			{
				return candidate;
			}
		}

		fallback.X = Mathf.Clamp(fallback.X, spawnBounds.Position.X, spawnBounds.End.X);
		fallback.Y = Mathf.Clamp(fallback.Y, spawnBounds.Position.Y, spawnBounds.End.Y);
		return fallback;
	}

	public bool ShouldRecycleEnemy(Vector2 enemyPosition, Vector2 playerPosition)
	{
		if (enemyPosition.DistanceTo(playerPosition) > EnemyRecycleDistance)
		{
			return true;
		}

		Rect2 allowedBounds = _worldBounds.Grow(-40f);
		return allowedBounds.Size != Vector2.Zero && !allowedBounds.HasPoint(enemyPosition);
	}

	public void HandleEnemyDefeated(Enemy enemy)
	{
		_kills += 1;

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
		enemy.SetSpawnPosition(GetEnemySpawnPosition(_player.GlobalPosition, boss));
		enemy.Activate();

		if (boss)
		{
			_bossAlive = true;
			_bossesSpawned += 1;
			SpawnBossEscortWave();
		}

		return true;
	}

	private Enemy RequestEnemyFromPool()
	{
		int randomEnemy = (int)(GD.Randi() % 2);
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
		return 1 + (int)(_runTime / 28f) + (_kills / 10) + _bossesSpawned * 2;
	}

	private int GetTargetEnemyCount()
	{
		int targetEnemies = BaseTargetEnemies + (int)(_runTime / 18f) + (_kills / 6) + _bossesSpawned;
		int capacityLimit = _enemyPoolCapacity > 0 ? _enemyPoolCapacity : MaxTargetEnemies;
		int maxAllowed = Mathf.Min(MaxTargetEnemies, capacityLimit);
		int minAllowed = Mathf.Min(BaseTargetEnemies, maxAllowed);
		return Mathf.Clamp(targetEnemies, minAllowed, maxAllowed);
	}

	private float GetCurrentSpawnInterval()
	{
		float spawnInterval = BaseSpawnInterval - _runTime * 0.02f - _kills * 0.012f;
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
		Vector2 direction = new Vector2(RandomFloat(-1f, 1f), RandomFloat(-1f, 1f)).Normalized();
		if (direction == Vector2.Zero)
		{
			direction = Vector2.Right;
		}

		return direction * RandomFloat(minRadius, maxRadius);
	}

	private float RandomFloat(float minValue, float maxValue)
	{
		return minValue + (maxValue - minValue) * GD.Randf();
	}

	private Vector2 RandomDirection()
	{
		Vector2 direction = new Vector2(RandomFloat(-1f, 1f), RandomFloat(-1f, 1f)).Normalized();
		return direction == Vector2.Zero ? Vector2.Right : direction;
	}

	private Rect2 GetSpawnBounds()
	{
		return _worldBounds.Size == Vector2.Zero ? _worldBounds : _worldBounds.Grow(-SpawnPadding);
	}

	private void CacheWorldBounds()
	{
		Node2D leftBoundary = GetNodeOrNull<Node2D>("LeftBoundary");
		Node2D rightBoundary = GetNodeOrNull<Node2D>("RightBoundary");
		Node2D topBoundary = GetNodeOrNull<Node2D>("TopBoundary");
		Node2D bottomBoundary = GetNodeOrNull<Node2D>("BottomBoundary");

		if (leftBoundary == null || rightBoundary == null || topBoundary == null || bottomBoundary == null)
		{
			return;
		}

		float left = leftBoundary.GlobalPosition.X;
		float right = rightBoundary.GlobalPosition.X;
		float top = topBoundary.GlobalPosition.Y;
		float bottom = bottomBoundary.GlobalPosition.Y;
		_worldBounds = new Rect2(new Vector2(left, top), new Vector2(right - left, bottom - top));
	}

	private void SpawnBossEscortWave()
	{
		int escorts = Mathf.Clamp(1 + _bossesSpawned, 1, 4);
		for (int i = 0; i < escorts && _activeEnemies < GetTargetEnemyCount(); i++)
		{
			SpawnConfiguredEnemy(false);
		}
	}

	private void ApplyScenePalette()
	{
		RenderingServer.SetDefaultClearColor(new Color(0.02f, 0.03f, 0.06f, 1f));
		CanvasItem tileMapLayer = GetNodeOrNull<CanvasItem>("TileMapLayer");
		if (tileMapLayer != null)
		{
			tileMapLayer.Modulate = new Color(0.52f, 0.58f, 0.7f, 0.92f);
		}
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
