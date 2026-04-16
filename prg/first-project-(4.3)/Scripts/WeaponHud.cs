using Godot;
using System;

public partial class WeaponHud : Control
{
	public WeaponInventory Inventory;

	private bool _inventoryOpen = false;
	private ColorRect _overlay;
	private HBoxContainer _bottomBar;
	private PanelContainer _menuPanel;
	private VBoxContainer _menuContent;
	private ProgressBar _nitroBar;
	private Label _nitroLabel;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		SetAnchorsPreset(LayoutPreset.FullRect);

		// Dark overlay
		_overlay = new ColorRect();
		_overlay.Color = new Color(0, 0, 0, 0.6f);
		_overlay.SetAnchorsPreset(LayoutPreset.FullRect);
		_overlay.Visible = false;
		_overlay.MouseFilter = MouseFilterEnum.Ignore;
		AddChild(_overlay);

		// Bottom weapon bar (always visible)
		_bottomBar = new HBoxContainer();
		_bottomBar.AnchorLeft = 0.5f;
		_bottomBar.AnchorRight = 0.5f;
		_bottomBar.AnchorTop = 1f;
		_bottomBar.AnchorBottom = 1f;
		_bottomBar.GrowHorizontal = GrowDirection.Both;
		_bottomBar.OffsetTop = -70;
		_bottomBar.OffsetBottom = -8;
		_bottomBar.OffsetLeft = -220;
		_bottomBar.OffsetRight = 220;
		_bottomBar.AddThemeConstantOverride("separation", 6);
		AddChild(_bottomBar);

		// Center menu panel (only when inventory open)
		_menuPanel = new PanelContainer();
		_menuPanel.AnchorLeft = 0.5f;
		_menuPanel.AnchorRight = 0.5f;
		_menuPanel.AnchorTop = 0.5f;
		_menuPanel.AnchorBottom = 0.5f;
		_menuPanel.GrowHorizontal = GrowDirection.Both;
		_menuPanel.GrowVertical = GrowDirection.Both;
		_menuPanel.OffsetLeft = -380;
		_menuPanel.OffsetRight = 380;
		_menuPanel.OffsetTop = -250;
		_menuPanel.OffsetBottom = 250;
		_menuPanel.Visible = false;

		var menuStyle = new StyleBoxFlat();
		menuStyle.BgColor = new Color(0.08f, 0.08f, 0.12f, 0.95f);
		menuStyle.SetBorderWidthAll(2);
		menuStyle.BorderColor = new Color(0.3f, 0.5f, 1f, 0.8f);
		menuStyle.SetCornerRadiusAll(16);
		menuStyle.SetContentMarginAll(20);
		_menuPanel.AddThemeStyleboxOverride("panel", menuStyle);
		AddChild(_menuPanel);

		_menuContent = new VBoxContainer();
		_menuContent.AddThemeConstantOverride("separation", 12);
		_menuPanel.AddChild(_menuContent);

		// Nitro bar (bottom-left)
		var nitroContainer = new HBoxContainer();
		nitroContainer.AnchorLeft = 0f;
		nitroContainer.AnchorRight = 0f;
		nitroContainer.AnchorTop = 1f;
		nitroContainer.AnchorBottom = 1f;
		nitroContainer.OffsetLeft = 12;
		nitroContainer.OffsetRight = 200;
		nitroContainer.OffsetTop = -36;
		nitroContainer.OffsetBottom = -10;
		nitroContainer.AddThemeConstantOverride("separation", 6);
		AddChild(nitroContainer);

		_nitroLabel = new Label();
		_nitroLabel.Text = "NITRO";
		_nitroLabel.AddThemeFontSizeOverride("font_size", 12);
		_nitroLabel.AddThemeColorOverride("font_color", new Color(0.2f, 0.9f, 1f));
		nitroContainer.AddChild(_nitroLabel);

		_nitroBar = new ProgressBar();
		_nitroBar.MinValue = 0;
		_nitroBar.MaxValue = 100;
		_nitroBar.Value = 100;
		_nitroBar.ShowPercentage = false;
		_nitroBar.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		_nitroBar.CustomMinimumSize = new Vector2(120, 20);

		var barBg = new StyleBoxFlat();
		barBg.BgColor = new Color(0.1f, 0.1f, 0.15f, 0.85f);
		barBg.SetCornerRadiusAll(4);
		_nitroBar.AddThemeStyleboxOverride("background", barBg);

		var barFill = new StyleBoxFlat();
		barFill.BgColor = new Color(0.1f, 0.85f, 1f, 0.9f);
		barFill.SetCornerRadiusAll(4);
		_nitroBar.AddThemeStyleboxOverride("fill", barFill);

		nitroContainer.AddChild(_nitroBar);

		CallDeferred(nameof(FindInventory));
	}

	private void FindInventory()
	{
		if (Inventory == null)
		{
			var game = GetTree().Root.GetNodeOrNull<Game>("Game");
			if (game != null)
				Inventory = game.GetNodeOrNull<WeaponInventory>("WeaponInventory");
		}
		if (Inventory != null)
		{
			Inventory.OnWeaponChanged += (_, __) => UpdateAll();
			UpdateAll();
		}

		// Hook nitro bar to player
		var player = GetTree().Root.GetNodeOrNull<Game>("Game")
			?.GetNodeOrNull<Player>("Player");
		if (player != null)
		{
			player.OnNitroChanged += (nitro) =>
			{
				_nitroBar.Value = nitro;
				var fill = _nitroBar.GetThemeStylebox("fill") as StyleBoxFlat;
				if (fill != null)
				{
					fill.BgColor = nitro < 20f
						? new Color(1f, 0.3f, 0.2f, 0.9f)
						: new Color(0.1f, 0.85f, 1f, 0.9f);
				}
			};
		}
	}

	public void SetInventoryOpen(bool open)
	{
		_inventoryOpen = open;
		_overlay.Visible = open;
		_menuPanel.Visible = open;
		UpdateAll();
	}

	private void UpdateAll()
	{
		UpdateBottomBar();
		if (_inventoryOpen) UpdateMenu();
	}

	private void UpdateBottomBar()
	{
		foreach (var child in _bottomBar.GetChildren())
			child.QueueFree();

		if (Inventory == null) return;

		for (int i = 0; i < Inventory.Weapons.Count; i++)
		{
			var weapon = Inventory.Weapons[i];
			bool selected = i == Inventory.CurrentIndex;

			var panel = new PanelContainer();
			panel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
			panel.CustomMinimumSize = new Vector2(80, 55);

			var style = new StyleBoxFlat();
			style.BgColor = selected
				? new Color(weapon.Color, 0.35f)
				: new Color(0.1f, 0.1f, 0.14f, 0.8f);
			style.BorderColor = selected
				? weapon.Color
				: new Color(0.3f, 0.3f, 0.3f, 0.5f);
			style.SetBorderWidthAll(selected ? 3 : 1);
			style.SetCornerRadiusAll(8);
			style.SetContentMarginAll(4);
			panel.AddThemeStyleboxOverride("panel", style);

			var hbox = new HBoxContainer();
			hbox.Alignment = BoxContainer.AlignmentMode.Center;
			hbox.AddThemeConstantOverride("separation", 4);

			// Small weapon icon
			var icon = new TextureRect();
			icon.CustomMinimumSize = new Vector2(28, 28);
			icon.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
			icon.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
			if (!string.IsNullOrEmpty(weapon.IconPath))
				icon.Texture = GD.Load<Texture2D>(weapon.IconPath);
			hbox.AddChild(icon);

			var vbox = new VBoxContainer();
			vbox.Alignment = BoxContainer.AlignmentMode.Center;

			var keyLabel = new Label();
			keyLabel.Text = $"[{i + 1}]";
			keyLabel.HorizontalAlignment = HorizontalAlignment.Center;
			keyLabel.AddThemeFontSizeOverride("font_size", 10);
			keyLabel.AddThemeColorOverride("font_color", new Color(0.7f, 0.7f, 0.7f));

			var nameLabel = new Label();
			nameLabel.Text = weapon.Name;
			nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
			nameLabel.AddThemeFontSizeOverride("font_size", selected ? 13 : 11);
			nameLabel.AddThemeColorOverride("font_color", selected ? Colors.White : new Color(0.8f, 0.8f, 0.8f));

			vbox.AddChild(keyLabel);
			vbox.AddChild(nameLabel);
			hbox.AddChild(vbox);
			panel.AddChild(hbox);
			_bottomBar.AddChild(panel);
		}
	}

	private void UpdateMenu()
	{
		foreach (var child in _menuContent.GetChildren())
			child.QueueFree();

		if (Inventory == null) return;

		// Title
		var title = new Label();
		title.Text = "INVENTORY";
		title.HorizontalAlignment = HorizontalAlignment.Center;
		title.AddThemeFontSizeOverride("font_size", 28);
		title.AddThemeColorOverride("font_color", new Color(0.4f, 0.7f, 1f));
		_menuContent.AddChild(title);

		// Separator
		var sep = new HSeparator();
		_menuContent.AddChild(sep);

		// Weapons grid
		var grid = new HBoxContainer();
		grid.Alignment = BoxContainer.AlignmentMode.Center;
		grid.AddThemeConstantOverride("separation", 16);
		_menuContent.AddChild(grid);

		for (int i = 0; i < Inventory.Weapons.Count; i++)
		{
			var weapon = Inventory.Weapons[i];
			bool selected = i == Inventory.CurrentIndex;

			var card = new PanelContainer();
			card.CustomMinimumSize = new Vector2(130, 300);

			var cardStyle = new StyleBoxFlat();
			cardStyle.BgColor = selected
				? new Color(weapon.Color, 0.2f)
				: new Color(0.12f, 0.12f, 0.18f, 0.9f);
			cardStyle.BorderColor = selected
				? weapon.Color
				: new Color(0.25f, 0.25f, 0.35f, 0.6f);
			cardStyle.SetBorderWidthAll(selected ? 3 : 1);
			cardStyle.SetCornerRadiusAll(12);
			cardStyle.SetContentMarginAll(10);
			card.AddThemeStyleboxOverride("panel", cardStyle);

			var cardVbox = new VBoxContainer();
			cardVbox.Alignment = BoxContainer.AlignmentMode.Center;
			cardVbox.AddThemeConstantOverride("separation", 8);

			// Key binding
			var keyLabel = new Label();
			keyLabel.Text = $"[{i + 1}]";
			keyLabel.HorizontalAlignment = HorizontalAlignment.Center;
			keyLabel.AddThemeFontSizeOverride("font_size", 14);
			keyLabel.AddThemeColorOverride("font_color", new Color(0.6f, 0.6f, 0.7f));
			cardVbox.AddChild(keyLabel);

			// Large weapon image
			var imgContainer = new CenterContainer();
			imgContainer.CustomMinimumSize = new Vector2(100, 100);
			var weaponImage = new TextureRect();
			weaponImage.CustomMinimumSize = new Vector2(90, 90);
			weaponImage.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
			weaponImage.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
			if (!string.IsNullOrEmpty(weapon.IconPath))
				weaponImage.Texture = GD.Load<Texture2D>(weapon.IconPath);
			imgContainer.AddChild(weaponImage);
			cardVbox.AddChild(imgContainer);

			// Weapon name
			var nameLabel = new Label();
			nameLabel.Text = weapon.Name;
			nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
			nameLabel.AddThemeFontSizeOverride("font_size", 16);
			nameLabel.AddThemeColorOverride("font_color", selected ? Colors.White : new Color(0.85f, 0.85f, 0.9f));
			cardVbox.AddChild(nameLabel);

			// Separator
			var cardSep = new HSeparator();
			cardVbox.AddChild(cardSep);

			// Stats
			var dmgLabel = new Label();
			dmgLabel.Text = $"DMG  {weapon.Damage}";
			dmgLabel.HorizontalAlignment = HorizontalAlignment.Center;
			dmgLabel.AddThemeFontSizeOverride("font_size", 12);
			dmgLabel.AddThemeColorOverride("font_color", new Color(1f, 0.5f, 0.4f));
			cardVbox.AddChild(dmgLabel);

			var rateLabel = new Label();
			rateLabel.Text = $"RATE {weapon.FireRate}/s";
			rateLabel.HorizontalAlignment = HorizontalAlignment.Center;
			rateLabel.AddThemeFontSizeOverride("font_size", 12);
			rateLabel.AddThemeColorOverride("font_color", new Color(0.4f, 0.8f, 1f));
			cardVbox.AddChild(rateLabel);

			// Equipped badge
			if (selected)
			{
				var equippedLabel = new Label();
				equippedLabel.Text = "EQUIPPED";
				equippedLabel.HorizontalAlignment = HorizontalAlignment.Center;
				equippedLabel.AddThemeFontSizeOverride("font_size", 11);
				equippedLabel.AddThemeColorOverride("font_color", weapon.Color);
				cardVbox.AddChild(equippedLabel);
			}

			card.AddChild(cardVbox);
			grid.AddChild(card);
		}

		// Hint
		var hint = new Label();
		hint.Text = "Press [1-5] to equip  |  [TAB] to close";
		hint.HorizontalAlignment = HorizontalAlignment.Center;
		hint.AddThemeFontSizeOverride("font_size", 13);
		hint.AddThemeColorOverride("font_color", new Color(0.5f, 0.5f, 0.6f));
		_menuContent.AddChild(hint);
	}
}
