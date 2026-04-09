using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node2D
{
	public int Score {
		get
		{
			if (int.TryParse(ScoreLabel.Text, out int score)) return score;
			return 0;
		}
			set => ScoreLabel.Text = value.ToString();
		}
	public static Game Instance { get; private set; }
	[Export]
	Godot.Collections.Dictionary<string, PoolObject> PoolObjects = new ();
	private Dictionary<string, Queue<Node>> _poolNodes = new ();
	private Dictionary<Type, string> _objectnames = new ();

	[Export] public Label ScoreLabel;
	
	public override void _Ready()
	{
		foreach(var keyValuePair in PoolObjects)
		{
			PoolObject poolObject = keyValuePair.Value;
			string name = keyValuePair.Key;
			Queue<Node> nodes = new ();
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
		int randomEnemy = GD.RandRange(0, 1);
		Enemy enemy;
		if (randomEnemy == 0)
		{
			enemy = GetPoolObject<MeleeEnemy>();
			if (enemy == null) enemy = GetPoolObject<RangeEnemy>();
		} else
		{
			enemy = GetPoolObject<RangeEnemy>();
			if (enemy == null) enemy = GetPoolObject<MeleeEnemy>();
		}
		
		if (enemy == null) return;
		enemy.Target = GetNode<Player>("Player");
		enemy.Activate();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
