using Godot;
using System;

public partial class HUD : CanvasLayer
{
	private Label _scoreLabel;
	private Label _waveLabel;
	private Label _killLabel;
	private ProgressBar _healthBar;
	private Panel _gameOverPanel;
	private Label _gameOverScore;
	private Label _waveAnnouncement;
	private float _waveAnnouncementTimer;
	private Label _dashCooldownLabel;

	public override void _Ready()
	{
		CreateUI();

		if (Game.Instance != null)
		{
			Game.Instance.OnScoreChanged += UpdateScore;
			Game.Instance.OnWaveChanged += UpdateWave;
			Game.Instance.OnKillCountChanged += UpdateKills;
		}

		var player = Game.Instance?.GetNodeOrNull<Player>("Player");
		if (player != null)
		{
			player.OnHealthChanged += UpdateHealth;
			UpdateHealth(player.Health);
		}
	}

	private void CreateUI()
	{
		// Top bar background
		var topBar = new Panel();
		var topStyle = new StyleBoxFlat();
		topStyle.BgColor = new Color(0, 0, 0, 0.6f);
		topStyle.CornerRadiusBottomLeft = 8;
		topStyle.CornerRadiusBottomRight = 8;
		topBar.AddThemeStyleboxOverride("panel", topStyle);
		topBar.SetAnchorsPreset(Control.LayoutPreset.TopWide);
		topBar.OffsetBottom = 65;
		AddChild(topBar);

		// Health bar
		_healthBar = new ProgressBar();
		_healthBar.ShowPercentage = false;
		_healthBar.OffsetLeft = 50;
		_healthBar.OffsetTop = 15;
		_healthBar.OffsetRight = 320;
		_healthBar.OffsetBottom = 42;
		_healthBar.MaxValue = 100;
		_healthBar.Value = 100;

		var bgStyle = new StyleBoxFlat();
		bgStyle.BgColor = new Color(0.2f, 0, 0);
		bgStyle.CornerRadiusBottomLeft = 4;
		bgStyle.CornerRadiusBottomRight = 4;
		bgStyle.CornerRadiusTopLeft = 4;
		bgStyle.CornerRadiusTopRight = 4;
		_healthBar.AddThemeStyleboxOverride("background", bgStyle);

		var fillStyle = new StyleBoxFlat();
		fillStyle.BgColor = new Color(0.8f, 0.1f, 0.1f);
		fillStyle.CornerRadiusBottomLeft = 4;
		fillStyle.CornerRadiusBottomRight = 4;
		fillStyle.CornerRadiusTopLeft = 4;
		fillStyle.CornerRadiusTopRight = 4;
		_healthBar.AddThemeStyleboxOverride("fill", fillStyle);
		AddChild(_healthBar);

		// Health label
		var healthLabel = new Label();
		healthLabel.Text = "HP";
		healthLabel.OffsetLeft = 24;
		healthLabel.OffsetTop = 14;
		healthLabel.OffsetRight = 50;
		healthLabel.OffsetBottom = 42;
		healthLabel.AddThemeColorOverride("font_color", Colors.White);
		healthLabel.AddThemeFontSizeOverride("font_size", 18);
		AddChild(healthLabel);

		// Dash hint
		_dashCooldownLabel = new Label();
		_dashCooldownLabel.Text = "[SHIFT] Dash";
		_dashCooldownLabel.OffsetLeft = 20;
		_dashCooldownLabel.OffsetTop = 44;
		_dashCooldownLabel.OffsetRight = 200;
		_dashCooldownLabel.OffsetBottom = 62;
		_dashCooldownLabel.AddThemeColorOverride("font_color", new Color(0.6f, 0.6f, 0.8f));
		_dashCooldownLabel.AddThemeFontSizeOverride("font_size", 14);
		AddChild(_dashCooldownLabel);

		// Score
		_scoreLabel = new Label();
		_scoreLabel.Text = "SCORE: 0";
		_scoreLabel.HorizontalAlignment = HorizontalAlignment.Right;
		_scoreLabel.SetAnchorsPreset(Control.LayoutPreset.TopRight);
		_scoreLabel.OffsetLeft = -250;
		_scoreLabel.OffsetTop = 8;
		_scoreLabel.OffsetRight = -20;
		_scoreLabel.OffsetBottom = 32;
		_scoreLabel.AddThemeColorOverride("font_color", new Color(1, 0.9f, 0.2f));
		_scoreLabel.AddThemeFontSizeOverride("font_size", 22);
		AddChild(_scoreLabel);

		// Wave
		_waveLabel = new Label();
		_waveLabel.Text = "WAVE: 0";
		_waveLabel.HorizontalAlignment = HorizontalAlignment.Center;
		_waveLabel.SetAnchorsPreset(Control.LayoutPreset.TopWide);
		_waveLabel.OffsetTop = 8;
		_waveLabel.OffsetBottom = 32;
		_waveLabel.AddThemeColorOverride("font_color", new Color(0.5f, 0.8f, 1.0f));
		_waveLabel.AddThemeFontSizeOverride("font_size", 22);
		AddChild(_waveLabel);

		// Kills
		_killLabel = new Label();
		_killLabel.Text = "KILLS: 0";
		_killLabel.HorizontalAlignment = HorizontalAlignment.Right;
		_killLabel.SetAnchorsPreset(Control.LayoutPreset.TopRight);
		_killLabel.OffsetLeft = -250;
		_killLabel.OffsetTop = 34;
		_killLabel.OffsetRight = -20;
		_killLabel.OffsetBottom = 58;
		_killLabel.AddThemeColorOverride("font_color", new Color(0.8f, 0.5f, 0.5f));
		_killLabel.AddThemeFontSizeOverride("font_size", 16);
		AddChild(_killLabel);

		// Wave announcement (center screen)
		_waveAnnouncement = new Label();
		_waveAnnouncement.HorizontalAlignment = HorizontalAlignment.Center;
		_waveAnnouncement.VerticalAlignment = VerticalAlignment.Center;
		_waveAnnouncement.SetAnchorsPreset(Control.LayoutPreset.Center);
		_waveAnnouncement.OffsetLeft = -300;
		_waveAnnouncement.OffsetTop = -50;
		_waveAnnouncement.OffsetRight = 300;
		_waveAnnouncement.OffsetBottom = 50;
		_waveAnnouncement.AddThemeColorOverride("font_color", Colors.White);
		_waveAnnouncement.AddThemeFontSizeOverride("font_size", 48);
		_waveAnnouncement.Visible = false;
		AddChild(_waveAnnouncement);

		// Game Over panel
		CreateGameOverPanel();
	}

	private void CreateGameOverPanel()
	{
		_gameOverPanel = new Panel();
		var style = new StyleBoxFlat();
		style.BgColor = new Color(0, 0, 0, 0.85f);
		_gameOverPanel.AddThemeStyleboxOverride("panel", style);
		_gameOverPanel.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		_gameOverPanel.Visible = false;
		AddChild(_gameOverPanel);

		var vbox = new VBoxContainer();
		vbox.SetAnchorsPreset(Control.LayoutPreset.Center);
		vbox.OffsetLeft = -200;
		vbox.OffsetTop = -150;
		vbox.OffsetRight = 200;
		vbox.OffsetBottom = 150;
		vbox.Alignment = BoxContainer.AlignmentMode.Center;
		_gameOverPanel.AddChild(vbox);

		var gameOverLabel = new Label();
		gameOverLabel.Text = "GAME OVER";
		gameOverLabel.HorizontalAlignment = HorizontalAlignment.Center;
		gameOverLabel.AddThemeColorOverride("font_color", new Color(1, 0.2f, 0.2f));
		gameOverLabel.AddThemeFontSizeOverride("font_size", 56);
		vbox.AddChild(gameOverLabel);

		_gameOverScore = new Label();
		_gameOverScore.Text = "Score: 0";
		_gameOverScore.HorizontalAlignment = HorizontalAlignment.Center;
		_gameOverScore.AddThemeColorOverride("font_color", new Color(1, 0.9f, 0.2f));
		_gameOverScore.AddThemeFontSizeOverride("font_size", 28);
		vbox.AddChild(_gameOverScore);

		var spacer = new Control();
		spacer.CustomMinimumSize = new Vector2(0, 30);
		vbox.AddChild(spacer);

		var restartButton = new Button();
		restartButton.Text = "RESTART";
		restartButton.CustomMinimumSize = new Vector2(200, 50);
		restartButton.AddThemeFontSizeOverride("font_size", 24);
		restartButton.Pressed += OnRestartPressed;
		vbox.AddChild(restartButton);
	}

	public override void _Process(double delta)
	{
		if (_waveAnnouncementTimer > 0)
		{
			_waveAnnouncementTimer -= (float)delta;
			float alpha = Math.Min(_waveAnnouncementTimer, 1.0f);
			_waveAnnouncement.Modulate = new Color(1, 1, 1, alpha);
			if (_waveAnnouncementTimer <= 0)
				_waveAnnouncement.Visible = false;
		}
	}

	private void UpdateScore(int score)
	{
		_scoreLabel.Text = $"SCORE: {score}";
	}

	private void UpdateWave(int wave)
	{
		_waveLabel.Text = $"WAVE: {wave}";
		if (wave > 0)
		{
			_waveAnnouncement.Text = $"WAVE {wave}";
			_waveAnnouncement.Visible = true;
			_waveAnnouncement.Modulate = Colors.White;
			_waveAnnouncementTimer = 2.5f;
		}
	}

	private void UpdateKills(int kills)
	{
		_killLabel.Text = $"KILLS: {kills}";
	}

	private void UpdateHealth(float health)
	{
		if (_healthBar == null) return;
		var player = Game.Instance?.GetNodeOrNull<Player>("Player");
		if (player != null)
		{
			_healthBar.MaxValue = player.MaxHealth;
			_healthBar.Value = health;
		}
	}

	public void ShowGameOver()
	{
		_gameOverPanel.Visible = true;
		_gameOverScore.Text = $"Score: {Game.Instance?.Score ?? 0}\nWave: {Game.Instance?.Wave ?? 0}\nKills: {Game.Instance?.KillCount ?? 0}";
	}

	public void HideGameOver()
	{
		_gameOverPanel.Visible = false;
	}

	private void OnRestartPressed()
	{
		Game.Instance?.RestartGame();
	}
}
