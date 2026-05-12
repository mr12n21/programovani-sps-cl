using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node2D
{
	public static Game Instance { get; private set; }
	private const string BossPoolPrefix = "boss:";

	[Export]
	Godot.Collections.Dictionary<string, PoolObject> PoolObjects = new();
	[Export] public float BaseSpawnInterval = 1.35f;
	[Export] public float MinimumSpawnInterval = 0.32f;
	[Export] public int BaseTargetEnemies = 6;
	[Export] public int MaxTargetEnemies = 24;
	[Export] public float BossSpawnEverySeconds = 28f;
	[Export] public int BossSpawnEveryKills = 12;
	[Export] public float DirectorRefillInterval = 0.2f;
	[Export] public float SpawnPadding = 220f;
	[Export] public float EnemyRecycleDistance = 1800f;
	[Export] public int AdditionalEnemyPoolPerType = 6;
	[Export] public int BossPoolSizePerType = 1;
	[Export] public int MaxConcurrentBosses = 1;
	[Export] public int MaxSpawnBurst = 5;

	private readonly Dictionary<string, Queue<Node>> _poolNodes = new();
	private readonly Dictionary<Type, string> _objectnames = new();
	private readonly Dictionary<Node, string> _nodePoolNames = new();
	private readonly List<string> _regularEnemyPoolNames = new();
	private readonly List<string> _bossEnemyPoolNames = new();
	private PackedScene _diamondPickupScene;
	private Timer _spawnTimer;
	private Player _player;
	private WeaponInventory _inventory;
	private WeaponHud _weaponHud;
	private int _activeRegularEnemies = 0;
	private int _activeBosses = 0;
	private int _kills = 0;
	private int _bossesSpawned = 0;
	private float _runTime = 0f;
	private float _refillCooldown = 0f;
	private Rect2 _worldBounds = new Rect2();
	private int _regularEnemyPoolCapacity = 0;
	private int _bossEnemyPoolCapacity = 0;
	
	public override void _Ready()
	{
		Instance = this;
		BuildPools();
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

	private void BuildPools()
	{
		foreach (var keyValuePair in PoolObjects)
		{
			PoolObject poolObject = keyValuePair.Value;
			if (poolObject?.Prefab == null || poolObject.PoolSize == 0)
			{
				continue;
			}

			InitializePool(keyValuePair.Key, poolObject);
		}
	}

	private void InitializePool(string poolName, PoolObject poolObject)
	{
		Node firstNode = poolObject.Prefab.Instantiate();
		if (firstNode is not IPoolable)
		{
			firstNode.Free();
			return;
		}

		bool enemyPool = firstNode is Enemy;
		int totalRegularNodes = (int)poolObject.PoolSize + (enemyPool ? AdditionalEnemyPoolPerType : 0);
		if (enemyPool)
		{
			_regularEnemyPoolNames.Add(poolName);
		}

		RegisterPooledNode(poolName, firstNode, enemyPool, false);
		for (int i = 1; i < totalRegularNodes; i++)
		{
			Node node = poolObject.Prefab.Instantiate();
			if (node is not IPoolable)
			{
				node.Free();
				break;
			}

			RegisterPooledNode(poolName, node, enemyPool, false);
		}

		if (!enemyPool || BossPoolSizePerType <= 0)
		{
			return;
		}

		string bossPoolName = GetBossPoolName(poolName);
		_bossEnemyPoolNames.Add(bossPoolName);
		for (int i = 0; i < BossPoolSizePerType; i++)
		{
			Node node = poolObject.Prefab.Instantiate();
			if (node is not IPoolable)
			{
				node.Free();
				break;
			}

			RegisterPooledNode(bossPoolName, node, true, true);
		}
	}

	private void RegisterPooledNode(string poolName, Node node, bool enemyNode, bool bossReserve)
	{
		if (!_poolNodes.TryGetValue(poolName, out Queue<Node> nodes))
		{
			nodes = new Queue<Node>();
			_poolNodes[poolName] = nodes;
		}

		nodes.Enqueue(node);
		_nodePoolNames[node] = poolName;
		if (!bossReserve)
		{
			_objectnames.TryAdd(node.GetType(), poolName);
		}

		if (enemyNode)
		{
			if (bossReserve)
			{
				_bossEnemyPoolCapacity += 1;
			}
			else
			{
				_regularEnemyPoolCapacity += 1;
			}
		}

		AddChild(node);
		((IPoolable)node).Init();
	}

	private static string GetBossPoolName(string poolName)
	{
		return $"{BossPoolPrefix}{poolName}";
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
		if (node == null)
		{
			return;
		}

		if (_nodePoolNames.TryGetValue(node, out string assignedPoolName))
		{
			EnqueuePoolObject(assignedPoolName, node);
			return;
		}

		Type nodeType = node.GetType();
		if (_objectnames.TryGetValue(nodeType, out string poolName))
		{
			EnqueuePoolObject(poolName, node);
			return;
		}

		if (_objectnames.TryGetValue(typeof(T), out poolName))
		{
			EnqueuePoolObject(poolName, node);
		}
	}

	public void SpawnEnemy()
	{
		EnsureReferences();
		if (_player == null)
		{
			return;
		}

		TrySpawnBoss();
		RefillRegularWave();
	}

	public override void _Process(double delta)
	{
		_runTime += (float)delta;
		_refillCooldown = Mathf.Max(0f, _refillCooldown - (float)delta);

		if (_spawnTimer != null)
		{
			_spawnTimer.WaitTime = GetCurrentSpawnInterval();
		}

		if (_refillCooldown <= 0f && NeedsDirectorRefill())
		{
			_refillCooldown = DirectorRefillInterval;
			SpawnEnemy();
		}

		UpdateDirectorStatus();
	}

	public void RegisterEnemySpawn(Enemy enemy)
	{
		if (enemy.IsBoss)
		{
			_activeBosses += 1;
		}
		else
		{
			_activeRegularEnemies += 1;
		}

		UpdateDirectorStatus();
	}

	public void RegisterEnemyReturned(Enemy enemy)
	{
		if (enemy.IsBoss)
		{
			_activeBosses = Mathf.Max(0, _activeBosses - 1);
		}
		else
		{
			_activeRegularEnemies = Mathf.Max(0, _activeRegularEnemies - 1);
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

		SpawnDiamondBurst(enemy.GlobalPosition, enemy.DiamondReward);
		UpdateDirectorStatus();
	}

	private void WarmupSpawn()
	{
		int warmupCount = Mathf.Min(GetTargetEnemyCount(), BaseTargetEnemies);
		for (int i = 0; i < warmupCount; i++)
		{
			if (!SpawnConfiguredEnemy(false))
			{
				break;
			}
		}
	}

	private bool SpawnConfiguredEnemy(bool boss)
	{
		EnsureReferences();
		Enemy enemy = RequestEnemyFromPool(boss);
		if (enemy == null || _player == null)
		{
			return false;
		}

		int intensity = GetDifficultyTier() + (boss ? 3 : 0);
		enemy.ConfigureSpawn(_player, intensity, boss);
		enemy.SetSpawnPosition(GetEnemySpawnPosition(_player.GlobalPosition, boss));
		enemy.Activate();

		if (boss)
		{
			_bossesSpawned += 1;
			SpawnBossEscortWave();
		}

		return true;
	}

	private Enemy RequestEnemyFromPool(bool boss)
	{
		List<string> poolNames = boss ? _bossEnemyPoolNames : _regularEnemyPoolNames;
		if (poolNames.Count == 0)
		{
			return null;
		}

		int startIndex = (int)(GD.Randi() % (uint)poolNames.Count);
		for (int offset = 0; offset < poolNames.Count; offset++)
		{
			string poolName = poolNames[(startIndex + offset) % poolNames.Count];
			if (GetPoolObject(poolName) is Enemy enemy)
			{
				return enemy;
			}
		}

		return null;
	}

	private void TrySpawnBoss()
	{
		if (ShouldSpawnBoss())
		{
			SpawnConfiguredEnemy(true);
		}
	}

	private void RefillRegularWave()
	{
		int missingEnemies = Mathf.Max(0, GetTargetEnemyCount() - _activeRegularEnemies);
		if (missingEnemies <= 0)
		{
			return;
		}

		int desiredBurst = 1 + GetDifficultyTier() / 3;
		int attempts = Mathf.Clamp(Mathf.Min(missingEnemies, desiredBurst), 1, MaxSpawnBurst);
		for (int i = 0; i < attempts; i++)
		{
			if (!SpawnConfiguredEnemy(false))
			{
				break;
			}
		}
	}

	private bool NeedsDirectorRefill()
	{
		return _activeRegularEnemies < GetTargetEnemyCount() || ShouldSpawnBoss();
	}

	private int GetDifficultyTier()
	{
		return 1 + (int)(_runTime / 20f) + (_kills / 8) + _bossesSpawned * 2;
	}

	private int GetTargetEnemyCount()
	{
		int targetEnemies = BaseTargetEnemies + (int)(_runTime / 12f) + (_kills / 4) + _bossesSpawned * 2;
		int capacityLimit = _regularEnemyPoolCapacity > 0 ? _regularEnemyPoolCapacity : MaxTargetEnemies;
		int maxAllowed = Mathf.Min(MaxTargetEnemies, capacityLimit);
		int minAllowed = Mathf.Min(BaseTargetEnemies, maxAllowed);
		return Mathf.Clamp(targetEnemies, minAllowed, maxAllowed);
	}

	private float GetCurrentSpawnInterval()
	{
		float spawnInterval = BaseSpawnInterval - _runTime * 0.03f - _kills * 0.018f - _bossesSpawned * 0.08f;
		return Mathf.Max(MinimumSpawnInterval, spawnInterval);
	}

	private bool ShouldSpawnBoss()
	{
		if (_activeBosses >= MaxConcurrentBosses || _bossEnemyPoolCapacity == 0)
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
		int escorts = Mathf.Clamp(2 + _bossesSpawned, 2, 6);
		int availableSlots = Mathf.Max(0, GetTargetEnemyCount() - _activeRegularEnemies);
		int spawnCount = Mathf.Min(escorts, availableSlots);
		for (int i = 0; i < spawnCount; i++)
		{
			if (!SpawnConfiguredEnemy(false))
			{
				break;
			}
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

		string status = _activeBosses > 0
			? $"BOSS WAVE  LVL {GetDifficultyTier()}  ENEMIES {_activeRegularEnemies}/{GetTargetEnemyCount()}  BOSS {_activeBosses}"
			: $"WAVE {GetDifficultyTier()}  ENEMIES {_activeRegularEnemies}/{GetTargetEnemyCount()}";
		_weaponHud.SetDirectorStatus(status);
	}
}
