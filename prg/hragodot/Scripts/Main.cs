using Godot;

public partial class Main : Node2D
{
    [Export] public PackedScene ItemScene { get; set; }
    [Export] public PackedScene EnemyScene { get; set; }
    [Export] public PackedScene LaserScene { get; set; }
    [Export] public float SpawnIntervalStart = 0.6f;
    [Export] public float SpawnIntervalMin = 0.18f;
    [Export] public float SpawnAcceleration = 0.03f;
    [Export] public float EnemySpawnIntervalStart = 1.4f;
    [Export] public float EnemySpawnIntervalMin = 0.6f;
    [Export] public float EnemySpawnAcceleration = 0.04f;
    [Export] public Vector2 SpawnXRange = new Vector2(40, 600);
    [Export] public float SpawnY = -20f;
    [Export] public Vector2 FallSpeedRange = new Vector2(220, 520);
    [Export] public Vector2 EnemySpeedRange = new Vector2(110, 200);
    [Export] public int ScorePerLevel = 20;
    [Export] public int StartLives = 3;

    private Timer _spawnTimer;
    private Timer _enemyTimer;
    private Label _scoreLabel;
    private Label _levelLabel;
    private Label _livesLabel;
    private Label _gameOverLabel;
    private Player _player;
    private readonly RandomNumberGenerator _rng = new();
    private int _score;
    private int _level = 1;
    private int _lives;
    private bool _gameOver;

    public override void _Ready()
    {
        _spawnTimer = GetNodeOrNull<Timer>("SpawnTimer") ?? CreateTimer("SpawnTimer", SpawnIntervalStart);
        _enemyTimer = GetNodeOrNull<Timer>("EnemyTimer") ?? CreateTimer("EnemyTimer", EnemySpawnIntervalStart);
        _player = GetNodeOrNull<Player>("Player");
        EnsureHud();

        if (ItemScene == null)
        {
            ItemScene = GD.Load<PackedScene>("res://Scenes/FallingItem.tscn");
        }
        if (EnemyScene == null)
        {
            EnemyScene = GD.Load<PackedScene>("res://Scenes/EnemyShip.tscn");
        }
        if (LaserScene == null)
        {
            LaserScene = GD.Load<PackedScene>("res://Scenes/Laser.tscn");
        }

        _rng.Randomize();
        _spawnTimer.WaitTime = SpawnIntervalStart;
        _enemyTimer.WaitTime = EnemySpawnIntervalStart;
        _spawnTimer.Timeout += OnSpawnTimerTimeout;
        _enemyTimer.Timeout += OnEnemyTimerTimeout;
        if (_player != null)
        {
            _player.Shoot += OnPlayerShoot;
        }

        _lives = StartLives;
        UpdateHud();
    }

    private void OnSpawnTimerTimeout()
    {
        if (_gameOver)
        {
            return;
        }

        SpawnItem();

        var next = Mathf.Max(SpawnIntervalMin, _spawnTimer.WaitTime - SpawnAcceleration);
        _spawnTimer.WaitTime = next;
    }

    private void OnEnemyTimerTimeout()
    {
        if (_gameOver)
        {
            return;
        }

        SpawnEnemy();

        var next = Mathf.Max(EnemySpawnIntervalMin, _enemyTimer.WaitTime - EnemySpawnAcceleration);
        _enemyTimer.WaitTime = next;
    }

    private void SpawnItem()
    {
        var item = ItemScene.Instantiate<FallingItem>();
        item.Position = new Vector2(_rng.RandfRange(SpawnXRange.X, SpawnXRange.Y), SpawnY);
        var levelScale = 1f + (_level - 1) * 0.08f;
        item.FallSpeed = _rng.RandfRange(FallSpeedRange.X, FallSpeedRange.Y) * levelScale;
        item.Type = _rng.Randf() < 0.7f ? FallingItem.ItemType.Asteroid : FallingItem.ItemType.Energy;
        item.RotationSpeed = _rng.RandfRange(-5f, 5f) * (item.Type == FallingItem.ItemType.Asteroid ? 1.4f : 0.6f);
        item.UpdateVisual();

        AddChild(item);

        item.Collected += OnItemCollected;
        item.PlayerHit += OnPlayerHit;
        item.Destroyed += OnItemDestroyed;
    }

    private void SpawnEnemy()
    {
        var enemy = EnemyScene.Instantiate<EnemyShip>();
        var levelScale = 1f + (_level - 1) * 0.1f;
        enemy.Position = new Vector2(_rng.RandfRange(SpawnXRange.X, SpawnXRange.Y), -40);
        enemy.Speed = _rng.RandfRange(EnemySpeedRange.X, EnemySpeedRange.Y) * levelScale;
        enemy.SwayAmplitude = _rng.RandfRange(20f, 70f);
        enemy.SwayFrequency = _rng.RandfRange(1.6f, 3.2f);
        enemy.Health = 1 + (_level / 3);
        enemy.Points = 4 + (_level / 2);

        AddChild(enemy);
        enemy.Destroyed += OnEnemyDestroyed;
        enemy.PlayerHit += OnPlayerHit;
    }

    private void OnItemCollected(int amount)
    {
        _score += amount;
        UpdateHud();
        CheckLevelUp();
    }

    private void OnItemDestroyed(int points)
    {
        _score += points;
        UpdateHud();
        CheckLevelUp();
    }

    private void OnEnemyDestroyed(int points)
    {
        _score += points;
        UpdateHud();
        CheckLevelUp();
    }

    private void OnPlayerHit(int damage)
    {
        if (_gameOver)
        {
            return;
        }

        _lives -= damage;
        if (_lives <= 0)
        {
            _gameOver = true;
            _spawnTimer?.Stop();
            _enemyTimer?.Stop();
            if (_gameOverLabel != null)
            {
                _gameOverLabel.Text = "Zásah! Stiskni R pro restart.";
            }
        }

        UpdateHud();
    }

    private void OnPlayerShoot(Vector2 position)
    {
        if (_gameOver)
        {
            return;
        }

        var laser = LaserScene.Instantiate<Laser>();
        laser.Position = position;
        AddChild(laser);
    }

    private void CheckLevelUp()
    {
        var target = _level * ScorePerLevel;
        if (_score >= target)
        {
            _level += 1;
            UpdateHud();
        }
    }

    private void UpdateHud()
    {
        if (_scoreLabel != null)
        {
            _scoreLabel.Text = $"Energie: {_score}";
        }
        if (_levelLabel != null)
        {
            _levelLabel.Text = $"Level: {_level}";
        }
        if (_livesLabel != null)
        {
            _livesLabel.Text = $"Štíty: {_lives}";
        }
    }

    private Timer CreateTimer(string name, float waitTime)
    {
        var timer = new Timer
        {
            Name = name,
            WaitTime = waitTime,
            Autostart = true
        };
        AddChild(timer);
        return timer;
    }

    private void EnsureHud()
    {
        var canvas = GetNodeOrNull<CanvasLayer>("CanvasLayer");
        if (canvas == null)
        {
            canvas = new CanvasLayer { Name = "CanvasLayer" };
            AddChild(canvas);
        }

        _scoreLabel = GetNodeOrNull<Label>("CanvasLayer/ScoreLabel") ?? CreateLabel(canvas, "ScoreLabel", new Vector2(16, 16), 24, "Energie: 0");
        _levelLabel = GetNodeOrNull<Label>("CanvasLayer/LevelLabel") ?? CreateLabel(canvas, "LevelLabel", new Vector2(16, 44), 18, "Level: 1");
        _livesLabel = GetNodeOrNull<Label>("CanvasLayer/LivesLabel") ?? CreateLabel(canvas, "LivesLabel", new Vector2(16, 68), 18, "Štíty: 3");
        _gameOverLabel = GetNodeOrNull<Label>("CanvasLayer/GameOverLabel") ?? CreateLabel(canvas, "GameOverLabel", new Vector2(16, 96), 20, "");
    }

    private Label CreateLabel(Node parent, string name, Vector2 position, int fontSize, string text)
    {
        var label = new Label
        {
            Name = name,
            Position = position,
            Text = text
        };
        label.Set("theme_override_font_sizes/font_size", fontSize);
        parent.AddChild(label);
        return label;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!_gameOver)
        {
            return;
        }

        if (@event is InputEventKey key && key.Pressed && !key.Echo && key.Keycode == Key.R)
        {
            GetTree().ReloadCurrentScene();
        }
    }
}
